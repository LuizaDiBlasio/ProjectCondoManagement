using ClassLibrary;
using ClassLibrary.DtoModels;
using CloudinaryDotNet.Actions;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Vereyon.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CondoManagementWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IFlashMessage _flashMessage;
        private readonly IConfiguration _configuration;
        private readonly IConverterHelper _converterHelper;
        private readonly CloudinaryService _cloudinaryService;
        private readonly HttpClient _httpClient;
        private readonly IApiCallService _apiCallService;


        public AccountController(IFlashMessage flashMessage, IConfiguration configuration, HttpClient httpClient,
            IConverterHelper converterHelper, CloudinaryService cloudinaryService, IApiCallService apiCallService)
        {

            _flashMessage = flashMessage;
            _configuration = configuration;
            _httpClient = httpClient;
            _cloudinaryService = cloudinaryService;
            _converterHelper = converterHelper;
            _apiCallService = apiCallService;

        }

        /// <summary>
        /// Displays the login view. If the user is already authenticated, redirects to the Home page.
        /// </summary>
        /// <returns>The login view or a redirection to the Home page.</returns>
        //Get do login 
        public IActionResult Login()
        {


            if (User.Identity.IsAuthenticated) //caso usuário esteja autenticado
            {
                if (User.IsInRole("SysAdmin"))
                {
                    return RedirectToAction("SysAdminDashBoard", "Account");
                }

                return RedirectToAction("Index", "Home"); //mandar para a view Index que possui o controller Home
            }

            return View(); //se login não funcionar, permanece na View 
        }


        /// <summary>
        /// Handles the login POST apiCall, authenticates the user, and redirects based on role.
        /// In case of success, Api returns a JWT wich will be stored as a cookie in the user session  
        /// </summary>
        /// <param name="model">The LoginViewModel containing user credentials.</param>
        /// <returns>A redirection to the appropriate page or the login view with error messages.</returns>
        [HttpPost]
        public async Task<IActionResult> RequestLogin(LoginViewModel model)
        {
            ////verificação para saber se a requisição é um AJAX POST
            //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        // Retorne um JSON para requisições AJAX que falham a validação
            //        return BadRequest(new { isSuccess = false, message = "Invalid input, please fill in the form correctly." });
            //    }
            //}
            //else // Requesição não AJAX (se você quiser manter o comportamento de form tradicional)
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        return View("Login", model);
            //    }
            //}

            //var loginDto = _converterHelper.ToLoginDto(model);
            //var jsonContent = new StringContent(
            //    JsonSerializer.Serialize(loginDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            //    Encoding.UTF8,
            //    "application/json"
            //);

            //try
            //{
            //    var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}api/Account/Login", jsonContent);

            ////    if (response.IsSuccessStatusCode)
            ////    {
            ////        var content = await response.Content.ReadAsStringAsync();
            ////        var tokenResponse = JsonSerializer.Deserialize<Response>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ////        //if (tokenResponse.Requires2FA) //TODO remover esse if antes de publicar
            ////        //{
            ////            // Para requisições AJAX, retorne um JSON que o JavaScript possa entender
            ////            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            ////            {
            ////                return Ok(new { isSuccess = true, requires2FA = true, username = model.Username });
            ////            }
            ////            // Para requisições não AJAX, volte a view
            ////            var responseModel = new LoginViewModel() { Username = model.Username, Requires2FA = true };

            ////            return View("Login", responseModel);
            ////        //}
            ////        //else // Login bem-sucedido (sem 2FA)
            ////        //{
            ////        //    //sessão com token
            ////        //    var handler = new JwtSecurityTokenHandler();
            ////        //    var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

            ////        //    //armazenar token na sessão para futuras requisições da api
            ////        //    HttpContext.Session.SetString("JwtToken", tokenResponse.Token);

            ////        //    // ClaimsPrincipal com base nas claims do token JWT
            ////        //    var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ////        //    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            ////        //    // login no  WebApp, criando um cookie de autenticação
            ////        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            ////        //    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            ////        //    {
            ////        //        return Ok(new { isSuccess = true, requires2FA = false });

            ////        //    }
            ////        //    return RedirectToAction("Index", "Home");
            ////        //}
            ////    }
            ////    else // Login falhou na API
            ////    {
            ////        var content2 = await response.Content.ReadAsStringAsync();
            ////        var error = JsonSerializer.Deserialize<Response>(content2, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ////        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            ////        {
            ////            return BadRequest(new { isSuccess = false, message = error.Message });
            ////        }

            ////        this.ModelState.AddModelError(string.Empty, error.Message);
            ////        return View("Login", model);
            ////    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            ////    {
            ////        return BadRequest(new { message = ex.Message });
            ////    }
            ////    return View("Login", model);
            ////}
            ///

            //______________________________novo Login_______________________

            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var loginDto = _converterHelper.ToLoginDto(model);
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(loginDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}api/Account/Login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<Response>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                    //armazenar token na sessão para futuras requisições da api
                    HttpContext.Session.SetString("JwtToken", tokenResponse.Token);

                    // ClaimsPrincipal com base nas claims do token JWT
                    var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // login no  WebApp, criando um cookie de autenticação
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                   
                    return RedirectToAction("Account", "SysAdminDashboard");
                }

                this.ModelState.AddModelError(string.Empty, "Unable to proceed with login");
                return View("Login", model);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return View("Login", model);
            }
        }

        /// <summary>
        /// Verifies sms code input from 2FA authentication
        /// </summary>
        /// <param name="model">model from modal partial view</param>
        /// <returns>Json response containing outcome</returns>
        [HttpPost]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid input for 2FA." });
            }

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(model, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}api/Account/Verify2FA", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<Response>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // **1. Recebe o tokenResponse COMPLETO da API**
                    if (tokenResponse.IsSuccess)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                        HttpContext.Session.SetString("JwtToken", tokenResponse.Token);

                        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        // **2. RETORNA o objeto COMPLETO, incluindo o Role, para o frontend**
                        return Ok(new { isSuccess = true, role = tokenResponse.Role });
                    }
                }
                else
                {
                    // Se a API retornar um erro (ex: código inválido)
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<Response>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return BadRequest(new { isSuccess = false, message = error?.Message ?? "Invalid verification code" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }

            return BadRequest(new { isSuccess = false, message = "An unknown error occurred." });
        }


        /// <summary>
        /// Displays ForgotPasswordPartial View
        /// </summary>
        /// <returns>IActionResult of the view</returns>
        //Get da _ForgotPasswordPartial
        public IActionResult ForgotPasswordPartial()
        {
            return PartialView("_ForgotPasswordPartial");
        }

        /// <summary>
        /// Call API to send a retrieve password link to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A json containing the API call outcome</returns>
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(model.Email, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json");

            try
            {
                var apiCall = await _httpClient.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}api/Account/GenerateForgotPasswordTokenAndEmail", jsonContent);

                if (apiCall.IsSuccessStatusCode)
                {
                    return Json(new { Message = "A retrieve password link has been sent to your email" });
                }

                return Json(new { Message = "Unable to send link, please contact admin." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Cleans Session, remove cookies and redirects to the Home page. 
        /// </summary>
        /// <returns>A redirection to the Home page.</returns>
        [Authorize(Roles = "CompanyAdmin, CondoManager, CondoMember, SysAdmin")]
        public async Task<IActionResult> Logout()
        {
            // Limpa o token JWT da sessão do Web App.
            HttpContext.Session.Clear();

            // Remove o cookie de autenticação do browser
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Volta para página inicial
            return RedirectToAction("Index", "Home");

        }

        /// <summary>
        /// Displays the user registration view. Only accessible by users with the "Admin" role.
        /// Populates available roles for selection.
        /// </summary>
        /// <returns>The registration view with available roles.</returns>
        [Authorize(Roles = "SysAdmin")]
        public IActionResult Register() //só mostra a view do Register
        {
            // modelo com as opções da combobox
            var selectList = new List<SelectListItem>
            {
                new SelectListItem{Value = "0", Text = "Select a role..."},
                new SelectListItem{Value = "CondoManager", Text = "Condo manager"},
                new SelectListItem{Value = "CondoMember", Text = "Condo member"},
                new SelectListItem{Value = "CompanyAdmin", Text = "Company admin"},
            };

            var model = new RegisterUserViewModel();

            model.AvailableRoles = selectList;

            model.Companies = new List<SelectListItem>();

            return View(model);
        }

        /// <summary>
        /// Makes an API Request to processe user registration
        /// Only users with the 'Admin' role can access this functionality.
        /// </summary>
        /// <param name="model">The model containing the data of the new user to be registered.</param>
        /// <returns>An action result indicating success or failure of the registration.</returns>
        [Authorize(Roles = "SysAdmin")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> RequestRegister(RegisterUserViewModel model) // registra o user
        {
            if (ModelState.IsValid) //ver se modelo é válido
            {

                if (model.SelectedRole == "0") // Se a opção "Select a role..." foi selecionada
                {
                    ModelState.AddModelError("SelectedRole", "You must select a valid role."); // Adiciona um erro específico para o campo SelectedRole


                    return View("Register", model); // Retorna a View com o erro
                }

                //Cloudnary da imagem

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var url = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                    model.ImageUrl = url;
                }

                //converter para dto
                var registerDto = _converterHelper.ToRegisterDto(model);

                //fazer chamada na api
                try
                {
                    var apiCall = await _apiCallService.PostAsync<RegisterUserDto, Response>("api/Account/Register", registerDto);

                    if (apiCall.IsSuccess)
                    {
                        _flashMessage.Confirmation(apiCall.Message);

                        return View("Register", new RegisterUserViewModel());
                    }
                    else // Login falhou na API
                    {
                        _flashMessage.Danger(apiCall.Message);

                        return View("Register", model);
                    }
                }
                catch (HttpRequestException e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _flashMessage.Danger("Access Unauthorized or session expired, please login again.");
                        return View("Register", model);
                    }
                    // outros erros HTTP de forma diferente
                    _flashMessage.Danger($"An HTTP error occurred: {e.Message}");
                    return View("Register", model);
                }
                catch (Exception e)
                {
                    // qualquer outro erro inesperado
                    _flashMessage.Danger($"An unexpected error occurred: {e.Message}");
                    return View("Register", model);
                }

            }
            // Se o result.Succeeded for false (login falhou )
            _flashMessage.Danger("An unexpected error occurred, user cannot be registered");

            return View("Register", model);
        }


        /// <summary>
        /// Displays the view for resetting the user's password after email confirmation.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="tokenEmail">The email confirmation token.</param>
        /// <returns>The password reset view or a "NotAuthorized" view if parameters are invalid.</returns>
        //Get do ResetPassword
        public async Task<IActionResult> ResetPassword(string userId, string tokenEmail)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenEmail)) //verificar parâmetros
            {
                return new NotFoundViewResult("NotAuthorized");
            }

            var model = new ResetPasswordViewModel()
            {
                UserId = userId,
                Token = tokenEmail,
                Password = " "  //ainda não foi colocada a senha
            };
            var resetPasswordDto = _converterHelper.ToResetPasswordDto(model);

            try
            {
                var apiCall = await _apiCallService.PostAsync<ResetPasswordDto, Response>("api/Account/GenerateResetPasswordToken", resetPasswordDto);

                if (apiCall.IsSuccess)
                {
                    var model2 = new ResetPasswordViewModel
                    {
                        Token = apiCall.Token,
                        UserId = userId,
                        Password = " " //ainda não foi colocada a senha
                    };


                    ModelState.Remove("Token"); //limpa o ModelState para evitar o uso do token antigo de confirmação de email

                    return View(model2);
                }

                return new NotFoundViewResult("NotAuthorized");
            }
            catch
            {
                return new NotFoundViewResult("NotAuthorized");
            }

        }

        /// <summary>
        /// Processes the user's password reset request and requests for SMS confirmation token.
        /// </summary>
        /// <param name="model">The model containing the username, reset token, and new password.</param>
        /// <returns>The password reset view with a success or error message.</returns>
        [HttpPost]
        public async Task<IActionResult> RequestResetPassword(ResetPasswordViewModel model) //recebo modelo preechido com dados para reset da password
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Token)) //verificar parâmetros (se o token for null, quer dizer que processo falhou e não autoriza)
            {
                return new NotFoundViewResult("NotAuthorized");
            }

            var resetPasswordDto = _converterHelper.ToResetPasswordDto(model); //esse já contém password

            try
            {
                var apiCall = await _apiCallService.PostAsync<ResetPasswordDto, Response>("api/Account/ResetPassword", resetPasswordDto);

                if (apiCall.IsSuccess)
                {
                    _flashMessage.Confirmation(apiCall.Message);

                    return View("ResetPassword", new ResetPasswordViewModel());
                }

                _flashMessage.Danger($"{apiCall.Message}");

                return View("ResetPassword", new ResetPasswordViewModel());
            }
            catch (Exception e)
            {
                _flashMessage.Danger($"{e.Message}");

                return View("ResetPassword", new ResetPasswordViewModel());
            }
        }


        /// <summary>
        /// Displays the view for recovering the user's password after email confirmation.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>The password recover view or a "NotAuthorized" view if parameters are invalid.</returns>
        //Get do RecoverPassword
        public IActionResult RecoverPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) //verificar parâmetros
            {
                return new NotFoundViewResult("NotAuthorized");
            }

            var model = new ResetPasswordViewModel()
            {
                UserId = userId,
                Token = token,
                Password = string.Empty  //ainda não foi colocada a senha
            };

            return View(model);
        }


        /// <summary>
        /// Processes the user's password recover request.
        /// </summary>
        /// <param name="model">The model containing the username, reset token, and new password.</param>
        /// <returns>The password recover view with a success or error message.</returns>
        [HttpPost]
        public async Task<IActionResult> RequestRecoverPassword(ResetPasswordViewModel model) //recebo modelo preechido com dados para recover da password
        {
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Token)) //verificar parâmetros (se o token for null, quer dizer que processo falhou e não autoriza)
            {
                return new NotFoundViewResult("NotAuthorized");
            }

            var resetPasswordDto = _converterHelper.ToResetPasswordDto(model); //esse já contém password

            var jsonContent = new StringContent(
               JsonSerializer.Serialize(model, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
               Encoding.UTF8,
               "application/json");

            try
            {
                var apiCall = await _httpClient.PostAsync("api/Account/ResetPassword", jsonContent);

                if (apiCall.IsSuccessStatusCode)
                {
                    _flashMessage.Confirmation("Password sucessfully reset");

                    return View("RecoverPassword", new RecoverPasswordViewModel());
                }

                _flashMessage.Danger($"Unable to reset password, please contact admin");

                return View("RecoverPassword", new RecoverPasswordViewModel());
            }
            catch (Exception e)
            {
                _flashMessage.Danger($"Unable to reset password, please contact admin");

                return View("RecoverPassword", new RecoverPasswordViewModel());
            }
        }



        /// <summary>
        /// Displays Change Password View
        /// </summary>
        /// <returns>IActionResult containing view ChangePassword</returns>
        public IActionResult ChangePassword()
        {
            return View();
        }


        /// <summary>
        /// Requests API to change Password
        /// </summary>
        /// <returns>View model containing outcome message</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var changePasswordDto = _converterHelper.ToChangePasswordDto(model);


            try
            {
                var apiCall = await _apiCallService.PostAsync<ChangePasswordDto, Response>("api/Account/ChangePassword", changePasswordDto);

                if (apiCall.IsSuccess)
                {
                    _flashMessage.Confirmation($"{apiCall.Message}");
                    return View("ChangePassword", model);
                }

                _flashMessage.Danger($"{apiCall.Message}");
                return View("ChangePassword", model);
            }
            catch (Exception e)
            {
                _flashMessage.Danger($"{e.Message}");
                return View("ChangePassword", model);
            }
        }

        /// <summary>
        /// Makes an http request to retrirve user's profile
        /// </summary>
        /// <returns>A view with user's profile details or empty if unsuccessfull</returns>
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var apicall = await _apiCallService.GetByQueryAsync<UserDto?>("api/Account/GetUserByEmail", this.User.Identity.Name);

                if (apicall == null)
                {
                    _flashMessage.Danger("Unable to retrive user's profile");
                    return View(new ProfileViewModel());
                }

                var model = _converterHelper.ToProfileViewModel(apicall);

                return View(model);
            }
            catch (Exception e)
            {
                _flashMessage.Danger("Unable to retrive user's profile");

                return View(new ProfileViewModel());
            }

        }


        /// <summary>
        /// Makes an http request to edit user's profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A view with user's profile details edited or view with errors i case call is unsuccessfull</returns>
        [Authorize]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            try
            {
                //buscar imagem do user
                var userDto = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", model.Email);

                if (userDto == null)
                {
                    _flashMessage.Danger("Unable to upload user's current picture");
                    return View("Profile", model);
                }

                model.ImageUrl = userDto.ImageUrl;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var url = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                    model.ImageUrl = url;
                }

                var userDto2 = _converterHelper.ToUserDto(model);

            
                var apiCall = await _apiCallService.PostAsync<UserDto, UserDto?>("api/Account/EditProfile", userDto2);

                if (apiCall == null)
                {
                    _flashMessage.Danger("Unable to edit user's profile");
                    return View("Profile", model);
                }

                var editedModel = _converterHelper.ToProfileViewModel(userDto2);

                return View("Profile", editedModel);
            }
            catch
            {
                _flashMessage.Danger("Unable to edit user's profile");
                return View("Profile", model);
            }

        }

        [Authorize]
        public async Task<IActionResult> SysAdminDashboard()
        {
            try
            {
                var model = new SysAdminDashboardViewModel();

                var usersCondoMembers = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CondoMember");

                if (usersCondoMembers.Any())
                {
                    model.CondoMembers = usersCondoMembers;
                }

                var usersCondoManagers = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CondoManagers");

                if (usersCondoManagers.Any())
                {
                    model.CondoManagers = usersCondoManagers;
                }


                var usersCompanyAdmin = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CompanyAdmin");

                if (usersCompanyAdmin.Any())
                {
                    model.CompanyAdmins = usersCompanyAdmin;
                }

                return View(model);

            }
            catch(Exception)
            {
                return View(new SysAdminDashboardViewModel());
            }

        }

        [Authorize]
        public async Task<IActionResult> EditUserDetails(string email)
        {
            try
            {
                var userdto = await _apiCallService.GetByQueryAsync<UserDto>($"api/Account/GetUserByEmail", email);


                if (userdto == null)
                {
                    var model1 = new EditUserDetailsViewModel();
                    _flashMessage.Danger("Unable to retrieve details, user not found");
                    return View(model1);
                }


                //Buscar a company name 

                if (userdto.CompanyId != null)
                {
                    var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/Details/{userdto.CompanyId}");

                    if(company == null) // se correu mal
                    {
                        _flashMessage.Danger("Unable to retrieve user's company");
                        var model2 = _converterHelper.ToEditUserDetailsViewModel(userdto, null);
                        return View(model2);
                    }

                    var model3 = _converterHelper.ToEditUserDetailsViewModel(userdto, company.Name);

                    return View(model3);
                }

                //se não tem company
                var model4 = _converterHelper.ToEditUserDetailsViewModel(userdto, null);
                return View(model4);

            }
            catch (Exception)
            {
                var model = new EditUserDetailsViewModel();
                _flashMessage.Danger("Unable to retrieve details, ser not found");
                return View(model);
            }
            
        }


        [Authorize]
        public async Task<IActionResult> RequestEditUserDetails(EditUserDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditUserDetails", model);
            }
            try
            {
                //buscar imagem do user
                var userDto = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", model.Email);

                if (userDto == null)
                {
                    _flashMessage.Danger("Unable to upload user's current picture");
                    return View("Profile", model);
                }

                model.ImageUrl = userDto.ImageUrl;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var url = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                    model.ImageUrl = url;
                }
 
                var editUserDetailsDto = _converterHelper.ToEditUserDetailsDto(model, model.CompanyName);

                var apiCall = await _apiCallService.PostAsync<EditUserDetailsDto, Response>("api/Account/EditUserDetails", editUserDetailsDto);

                if (apiCall.IsSuccess)
                {
                    var editedUserDto = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", model.Email);

                    if (editedUserDto != null)
                    {
                        if (editedUserDto.CompanyId != null)
                        {
                            var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/Details/{editedUserDto.CompanyId}");

                            if (company == null) // se correu mal
                            {
                                _flashMessage.Danger("Unable to retrieve user's company");
                                var model2 = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, null);
                                return View("EditUserDetails", model2);
                            }

                            var editedUserDetailsViewModel = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, company.Name);

                            return View("EditUserDetails", editedUserDetailsViewModel);
                        }

                        var model3 = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, null);
                        return View("EditUserDetails", model3);

                    }

                    _flashMessage.Danger(apiCall.Message);
                    var model4 = new EditUserDetailsViewModel();
                    return View("EditUserDetails", model4);
                }

                _flashMessage.Danger("An unexpected error occurred, unable to update user's details");
                var model5 = new EditUserDetailsViewModel();
                return View("EditUserDetails", model5);
            }
            catch (Exception)
            {
                _flashMessage.Danger("An unexpected error occurred, unable to update user's details");
                var model6 = new EditUserDetailsViewModel();
                return View("EditUserDetails", model6);
            }

        }


        //[Authorize]
        //public async Task<IActionResult> RequestEditUserDetails(EditUserDetailsViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("EditUserDetails", model);
        //    }
        //    try
        //    {
        //        var editUserDetailsDto = _converterHelper.ToEditUserDetailsDto(model, model.CompanyName);

        //        var editedUserDto = await _apiCallService.PostAsync<EditUserDetailsDto, UserDto>("api/Account/EditUserDetails", editUserDetailsDto);

        //        if (editedUserDto != null)
        //        {
        //            if (editedUserDto.CompanyId != null)
        //            {
        //                var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/Details/{editedUserDto.CompanyId}");

        //                if (company == null) // se correu mal
        //                {
        //                    _flashMessage.Danger("Unable to retrieve user's company");
        //                    var model1 = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, null);
        //                    return View("EditUserDetails", model1);
        //                }

        //                var editedUserDetailsViewModel = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, company.Name);

        //                return View(editedUserDetailsViewModel);
        //            }

        //            var model2 = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, null);
        //            return View("EditUserDetails", model2);
        //        }

        //        _flashMessage.Danger("An unexpected error occurred, unable to update user's details");
        //        var model3 = new EditUserDetailsViewModel();
        //        return View("EditUserDetails", model3);
        //    }
        //    catch (Exception)
        //    {
        //        _flashMessage.Danger("An unexpected error occurred, unable to update user's details");
        //        var model4 = new EditUserDetailsViewModel();
        //        return View("EditUserDetails", model4);
        //    }

        //}



        // <summary>
        /// Displays the "Not Authorized" view when a user tries to access a restricted area.
        /// </summary>
        /// <returns>The "Not Authorized" view.</returns>
        public IActionResult NotAuthorized()
        {
            return View();
        }


        /// <summary>
        /// Displays the "User Not Found" view when a user cannot be located.
        /// </summary>
        /// <returns>The "User Not Found" view.</returns>
        public IActionResult UserNotFound()
        {
            return View();
        }
    }
}




