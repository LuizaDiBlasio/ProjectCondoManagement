using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Vereyon.Web;

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



        //________________________________________________________________________________________________________________________________________________________________

        /// <summary>
        /// Handles the login POST apiCall, authenticates the user, and redirects based on role.
        /// In case of success, Api returns a JWT wich will be stored as a cookie in the user session  
        /// </summary>
        /// <param name="model">The LoginViewModel containing user credentials.</param>
        /// <returns>A redirection to the appropriate page or the login view with error messages.</returns>
        [HttpPost]
        public async Task<IActionResult> RequestLogin(LoginViewModel model)
        {
            //verificação para saber se a requisição é um AJAX POST
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (!ModelState.IsValid)
                {
                    // Retorne um JSON para requisições AJAX que falham a validação
                    return BadRequest(new { isSuccess = false, message = "Invalid input, please fill in the form correctly." });
                }
            }
            else // Requesição não AJAX (se você quiser manter o comportamento de form tradicional)
            {
                if (!ModelState.IsValid)
                {
                    return View("Login", model);
                }
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
                    var tokenResponse = JsonSerializer.Deserialize<ClassLibrary.Response<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (tokenResponse.Requires2FA)
                    {
                        //limpar os cookies da sessão anterior
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        // Para requisições AJAX, retorne um JSON que o JavaScript possa entender
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Ok(new { isSuccess = true, requires2FA = true, username = model.Username });
                        }

                        var responseModel = new LoginViewModel() { Username = model.Username, Requires2FA = true };

                        return View("Login", responseModel);
                    }
                    else // Login bem-sucedido (sem 2FA) //TODO remover esse else antes de publicar
                    {
                        //limpar os cookies da sessão anterior
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        //sessão com token
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                        //armazenar token na sessão para futuras requisições da api
                        HttpContext.Session.SetString("JwtToken", tokenResponse.Token);

                        // ClaimsPrincipal com base nas claims do token JWT
                        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // login no  WebApp, criando um cookie de autenticação
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Ok(new { isSuccess = true, requires2FA = false });

                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                else // Login falhou na API
                {
                    var content2 = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ClassLibrary.Response<object>>(content2, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return BadRequest(new { isSuccess = false, message = error.Message });
                    }

                    this.ModelState.AddModelError(string.Empty, error.Message);
                    return View("Login", model);
                }
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { message = ex.Message });
                }
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
                    var tokenResponse = JsonSerializer.Deserialize<ClassLibrary.Response<object>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
                    var error = JsonSerializer.Deserialize<ClassLibrary.Response<object>>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
        [Authorize]
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
        public async Task<IActionResult> Register() //só mostra a view do Register
        {
            // modelo com as listas

            var rolesSelectList = GetRolesSelectList();

            var companiesList = await _apiCallService.GetAsync<List<CompanyDto>>("api/Company/GetCompanies");

            var comaniesSelectList = _converterHelper.ToCompaniesSelectList(companiesList);

            var model = new RegisterUserViewModel();

            model.AvailableRoles = rolesSelectList;

            model.AvailableCompanies = comaniesSelectList;


            if (!companiesList.Any())
            {
                _flashMessage.Warning("You need to add companies to de system, before registering user");

                var model2 = new CreateEditCompanyViewModel();

                return View("~/Views/Company/Create.cshtml", model2);
            }

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
                // Validação manual para a seleção de empresa
                if (model.SelectedRole == "CompanyAdmin" || model.SelectedRole == "CondoManager")
                {
                    if (!model.SelectedCompanyId.HasValue)
                    {
                        await LoadModelLists(model);

                        ModelState.AddModelError("SelectedCompanyId", "You need to select a company.");

                        return View("Register", model);
                    }
                }
                else if (model.SelectedRole == "CondoMember")
                {
                    if (model.SelectedCompaniesIds == null || !model.SelectedCompaniesIds.Any())
                    {
                        await LoadModelLists(model);

                        ModelState.AddModelError("SelectedCompaniesIds", "You need to select at least one company.");

                        return View("Register", model);
                    }
                }


                if (model.SelectedRole == "0") // Se a opção "Select a role..." foi selecionada
                {
                    ModelState.AddModelError("SelectedRole", "You must select a valid role."); // Adiciona um erro específico para o campo SelectedRole  

                    await LoadModelLists(model);

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

                registerDto.Companies = new List<CompanyDto>();

                //adicionar companies
                if (model.SelectedRole == "CompanyAdmin" || model.SelectedRole == "CondoManager")
                {
                    if (model.SelectedCompanyId.HasValue)
                    {
                        var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{model.SelectedCompanyId.Value}");
                        if (company != null)
                        {
                            registerDto.Companies.Add(company);
                        }
                    }
                }
                else if (model.SelectedRole == "CondoMember")
                {
                    foreach (var companyId in model.SelectedCompaniesIds)
                    {
                        var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{companyId}");
                        if (company != null)
                        {
                            registerDto.Companies.Add(company);
                        }
                    }
                }

                //fazer chamada na api
                try
                {
                    var apiCall = await _apiCallService.PostAsync<RegisterUserDto, ClassLibrary.Response<object>>("api/Account/Register", registerDto);

                    if (apiCall.IsSuccess)
                    {

                        _flashMessage.Confirmation(apiCall.Message);

                        return RedirectToAction(nameof(SysAdminDashboard));
                    }
                    else // Login falhou na API
                    {
                        _flashMessage.Danger(apiCall.Message);

                        await LoadModelLists(model);

                        return View("Register", model);
                    }
                }
                catch (HttpRequestException e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _flashMessage.Danger("Access Unauthorized or session expired, please login again.");

                        await LoadModelLists(model);

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

                    await LoadModelLists(model);

                    return View("Register", model);
                }

            }
            // Se o result.Succeeded for false (login falhou )
            _flashMessage.Danger("An unexpected error occurred, user cannot be registered");

            await LoadModelLists(model);

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
                var apiCall = await _apiCallService.PostAsync<ResetPasswordDto, ClassLibrary.Response<object>>("api/Account/GenerateResetPasswordToken", resetPasswordDto);

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
                var apiCall = await _apiCallService.PostAsync<ResetPasswordDto, ClassLibrary.Response<object>>("api/Account/ResetPassword", resetPasswordDto);

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
        [Authorize]
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
                var apiCall = await _apiCallService.PostAsync<ChangePasswordDto, ClassLibrary.Response<object>>("api/Account/ChangePassword", changePasswordDto);

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

                userDto2.CompaniesDto = await _apiCallService.GetAsync<List<CompanyDto>>($"api/Company/GetCompanyByUser"); 

                var apiCall = await _apiCallService.PostAsync<UserDto, UserDto?>("api/Account/EditProfile", userDto2);

                if (apiCall == null)
                {
                    _flashMessage.Danger("Unable to edit user's profile");
                    return View("Profile", model);
                }

                var editedModel = _converterHelper.ToProfileViewModel(userDto2);
                if (this.User.IsInRole("CondoMember"))
                {
                    _flashMessage.Confirmation("Profile updated successfully.");
                    return RedirectToAction("Dashboard", "CondoMember");
                }

                return View("Profile", editedModel);
            }
            catch(Exception ex)
            {
                _flashMessage.Danger("Unable to edit user's profile");
                return View("Profile", model);
            }

        }


        /// <summary>
         /// Renders the SysAdmin dashboard by retrieving a list of users for each administrative role.
         /// </summary>
         /// <returns>
         /// An IActionResult that returns the SysAdminDashboard view with the populated user data.
         /// Returns an empty model on a caught exception.
         /// </returns>
        [Authorize(Roles = "SysAdmin")]
        public async Task<IActionResult> SysAdminDashboard()
        {

            var model = new SysAdminDashboardViewModel();

            try
            {


                var usersCondoMembers = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CondoMember");

                if (usersCondoMembers.Any())
                {
                    model.CondoMembers = usersCondoMembers;
                }

                var usersCompanyAdmin = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CompanyAdmin");

                if (usersCompanyAdmin.Any())
                {
                    model.CompanyAdmins = usersCompanyAdmin;
                }

                var usersCondoManagers = await _apiCallService.GetAsync<IEnumerable<UserDto>>($"api/Account/GetUsersWithCompany?role=CondoManager");

                if (usersCondoManagers.Any())
                {
                    model.CondoManagers = usersCondoManagers;
                }



                return View(model);

            }
            catch (Exception ex)
            {
                return View(model);
            }

        }


        /// <summary>
         /// Renders the user details edit view, retrieving user information by email.
         /// </summary>
         /// <param name="email">The email of the user to retrieve details for.</param>
         /// <returns>
         /// An IActionResult that returns the EditUserDetails view populated with the user's data.
         /// Returns an empty model and an error message if the user or their company cannot be found.
         /// </returns>
        [Authorize(Roles = "SysAdmin")]
        public async Task<IActionResult> EditUserDetails(string email)
        {
            try
            {
                var userdto = await _apiCallService.GetByQueryAsync<UserDto>($"api/Account/GetUserByEmail", email);

               
                if (userdto == null)
                {
                    var model = new SysAdminDashboardViewModel();

                    await LoadDashboardDataAsync(model);

                    _flashMessage.Danger("Unable to retrieve details, user not found");

                    return View("SysAdminDashBoard", model);
                }


                if (userdto.CompaniesDto.Any())
                {
                    var model = await LoadEditUserDetailsViewModel(userdto);              
                    return View(model);
                }

                var companiesSelectList2 = await LoadCompaniesSelectList();

                var model2 = _converterHelper.ToEditUserDetailsViewModel(userdto, companiesSelectList2, null, null);

                return View(model2);

            }
            catch (Exception)
            {
                var model = LoadEditUserModel();
                _flashMessage.Danger("Unable to retrieve details, ser not found");
                return View(model);
            }

        }


        /// <summary>
         /// Handles the form submission for updating a user's details, including their profile picture.
         /// </summary>
         /// <param name="model">The view model containing the updated user details.</param>
         /// <returns>
         /// An IActionResult that redirects to the EditUserDetails view on success.
         /// Returns the same view with an error message on failure or validation errors.
         /// </returns>

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

                var editUserDetailsDto = _converterHelper.ToEditUserDetailsDto(model);

                //escolher companies 
                if ( model.UserRole == "CompanyAdmin" || model.UserRole == "CondoManager")
                {
                    if (model.SelectedCompanyId.HasValue)
                    {
                        var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{model.SelectedCompanyId.Value}");
                        if (company != null)
                        {
                            editUserDetailsDto.CompaniesDto.Add(company);
                        }
                    }
                }
                else if (model.UserRole == "CondoMember")
                {
                    foreach (var companyId in model.SelectedCompaniesIds)
                    {
                        var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{companyId}");
                        if (company != null)
                        {
                            editUserDetailsDto.CompaniesDto.Add(company);
                        }
                    }
                }

                var apiCall = await _apiCallService.PostAsync<EditUserDetailsDto, ClassLibrary.Response<object>>("api/Account/EditUserDetails", editUserDetailsDto);

                if (apiCall.IsSuccess)
                {
                    var editedUserDto = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", model.Email);

                    if (editedUserDto != null)
                    {
                        if (editedUserDto.CompaniesDto.Any())
                        {
                            _flashMessage.Confirmation("User details updated successfully!");
                            return RedirectToAction(nameof(SysAdminDashboard));

                        }
                        _flashMessage.Danger("Unable to select user's company, please contact admin");

                        var companiesSelectList1 = await LoadCompaniesSelectList();

                        var model3 = _converterHelper.ToEditUserDetailsViewModel(editedUserDto, companiesSelectList1, null, null);

                        return View("EditUserDetails", model3);

                    }

                    _flashMessage.Danger("Unable to update user details");
                    var companiesSelectList2 = await LoadCompaniesSelectList();

                    model.AvailableCompanies = companiesSelectList2;

                    return View("EditUserDetails", model);
                }

                _flashMessage.Danger(apiCall.Message);
                var companiesSelectList3 = await LoadCompaniesSelectList();

                model.AvailableCompanies = companiesSelectList3;

                return View("EditUserDetails", model);
            }
            catch (Exception)
            {
                _flashMessage.Danger("An unexpected error occurred, unable to update user's details");
                var companiesSelectList = await LoadCompaniesSelectList();

                model.AvailableCompanies = companiesSelectList;

                return View("EditUserDetails", model);
            }

        }


        /// <summary>
         /// Handles user searches from the SysAdmin dashboard.
         /// </summary>
         /// <param name="model">The view model containing the search term.</param>
         /// <returns>
         /// An IActionResult that redirects to the EditUserDetails view if a single user is found.
         /// Returns the SysAdminDashboard view with a list of matching users if multiple are found, or an error message if none are found.
         /// </returns>
        [Authorize(Roles = "SysAdmin")]
        [HttpPost]
        public async Task<IActionResult> SearchUsers(SysAdminDashboardViewModel model)
        {

            if (string.IsNullOrEmpty(model.SearchTerm))
            {
                _flashMessage.Danger("Please provide a search term.");

                await LoadDashboardDataAsync(model); // Recarrega os dados da dashboard antes de retornar a view, metodo auxiliar abaixo
                return View("SysAdminDashboard", model);
            }

            try
            {
                // Chamada à API para buscar usuários por nome
                var users = await _apiCallService.GetAsync<List<UserDto>>($"api/Account/GetUsersByFullName?userFullName={model.SearchTerm}"); //criar uma query string pra passar o parametro

                if (users == null || !users.Any())
                {
                    _flashMessage.Danger("No users found with that name.");
                    await LoadDashboardDataAsync(model); // Recarrega os dados
                    return View("SysAdminDashboard", model);
                }

                if (users.Count == 1)
                {
                    var user = users.First(); // puxa o primeiro e único da lista


                    //criar view model para EditUserDetails
                    if (user.CompaniesDto.Any())
                    {
                        var editedUserDetailsViewModel = LoadEditUserDetailsViewModel(user);

                        return RedirectToAction("EditUserDetails", editedUserDetailsViewModel);
                    }

                    var companiesList = await LoadCompaniesSelectList();

                    var model3 = _converterHelper.ToEditUserDetailsViewModel(user, companiesList, null, null);

                    _flashMessage.Warning("Unable to retrieve user's companies, please contact admin");

                    return RedirectToAction("EditUserDetails", model3);
                }
                else
                {
                    model.HomonymUsers = users;
                    await LoadDashboardDataAsync(model); // Recarrega os dados
                    return View("SysAdminDashboard", model);

                }

            }
            catch
            {
                _flashMessage.Danger("Unable to search due to error");

                await LoadDashboardDataAsync(model);
                return View("SysAdminDashboard", model);
            }

        }

        /// <summary>
        /// Auxiliar method for SearchUsers(); Loads the 3 lists of users
        /// </summary>
        /// <param name="model">View model</param>
        /// <returns>Task</returns>
        [Authorize(Roles = "SysAdmin")]
        private async Task LoadDashboardDataAsync(SysAdminDashboardViewModel model)
        {
            model.CondoMembers = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CondoMember");
            model.CompanyAdmins = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CompanyAdmin");
            model.CondoManagers = await _apiCallService.GetByQueryAsync<IEnumerable<UserDto>>("api/Account/GetAllUsersByRole", "CondoManagers");
        }




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







        public async Task<IActionResult> AssignCompany(string? email)
        {

            if (email == null)
            {
                return NotFound();
            }

            try
            {
                var user = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", email);
                if (user == null)
                {
                    return NotFound();
                }

                var model = await LoadAssingCompanyViewModel(user);

                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger($"Unable to assing company to user, please contact admin");
                return RedirectToAction(nameof(SysAdminDashboard));
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCompany(string? email, AssignCompanyViewModel model)
        {

            if (email == null)
            {
                return NotFound();
            }


            if (!model.CompaniesDto.Any())
            {
                _flashMessage.Danger("select one company.");
                return View(model);
            }
            
            try
            {
                var user = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", email);
                if (user == null)
                {
                    return NotFound();
                }

                var editUserDto = new EditUserDetailsDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    ImageUrl = user.ImageUrl,
                    BirthDate = user.BirthDate,
                    FinancialAccountId = user.FinancialAccountId,
                    IsActive = user.IsActive,
                    Uses2FA = user.Uses2FA,
                };

                //escolher companies 
                if (model.UserRole == "CompanyAdmin" || model.UserRole == "CondoManager")
                {
                    if (model.SelectedCompanyId.HasValue)
                    {
                        var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{model.SelectedCompanyId.Value}");
                        if (company != null)
                        {
                            editUserDto.CompaniesDto.Add(company);

                            //adicionar à company
                            company.Users.Add(user);

                            if(model.UserRole == "CompanyAdmin")
                            {
                                company.CompanyAdminId = user.Id;

                                //update da company
                                var apiCall = await _apiCallService.PostAsync<CompanyDto, ClassLibrary.Response<object>>("api/Company/EditCompany", company);
                            }
                        }
                    }
                }
                else if (model.UserRole == "CondoMember")
                {
                    foreach (var companyId in model.SelectedCompaniesIds)
                    {
                        var company = await _apiCallService.GetAsync<CompanyDto>($"api/Company/GetCompany/{companyId}");
                        if (company != null)
                        {
                            editUserDto.CompaniesDto.Add(company);

                            //adicionar à company
                            company.Users.Add(user);

                            //update da company
                            var apiCall = await _apiCallService.PostAsync<CompanyDto, ClassLibrary.Response<object>>("api/Company/EditCompany", company);
                        }
                    }
                }

                

                //update do user
                var result = await _apiCallService.PostAsync<EditUserDetailsDto, ClassLibrary.Response<object>>("api/Account/EditUserDetails", editUserDto);

                _flashMessage.Confirmation("Company assigned successfully!");
                return RedirectToAction(nameof(SysAdminDashboard));
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error assigning Company!");
            }

            return RedirectToAction(nameof(SysAdminDashboard));

        }

        public List<SelectListItem> GetRolesSelectList()
        {
            return new List<SelectListItem>
            {
                 new SelectListItem{Value = "0", Text = "Select a role..."},
                 new SelectListItem{Value = "CondoManager", Text = "Condo manager"},
                 new SelectListItem{Value = "CondoMember", Text = "Condo member"},
                 new SelectListItem{Value = "CompanyAdmin", Text = "Company admin"},
            };
        }

        public async Task LoadModelLists(RegisterUserViewModel model)
        {
            var rolesSelectList = GetRolesSelectList();

            var companiesList = await _apiCallService.GetAsync<List<CompanyDto>>("api/Company/GetCompanies");

            var comaniesSelectList = _converterHelper.ToCompaniesSelectList(companiesList);

            model.AvailableRoles = rolesSelectList;

            model.AvailableCompanies = comaniesSelectList;
        }
             
        public async Task<EditUserDetailsViewModel> LoadEditUserModel()
        {
            var model = new EditUserDetailsViewModel();

            var companiesList = await _apiCallService.GetAsync<List<CompanyDto>>("api/Company/GetCompanies");

            var companiesSelectList = _converterHelper.ToCompaniesSelectList(companiesList);

            model.AvailableCompanies = companiesSelectList;

            return model;   
        }

        public async Task<List<SelectListItem>> LoadCompaniesSelectList()
        {
            var companiesList = await _apiCallService.GetAsync<List<CompanyDto>>("api/Company/GetCompanies");

            var companiesSelectList = _converterHelper.ToCompaniesSelectList(companiesList);

            return companiesSelectList; 
        }

        public async Task<EditUserDetailsViewModel> LoadEditUserDetailsViewModel(UserDto user)
        {
            //carregar lista
            var companiesList1 = await LoadCompaniesSelectList();

            //carregar model
            var model = _converterHelper.ToEditUserDetailsViewModel(user, companiesList1, null, null);

            //carregar companies do user 
            var userRole = await _apiCallService.GetAsync<string>($"api/Account/GetUserRole/{user.Email}");

            //carregar userRole
            if (userRole != null)
            {
                model.UserRole = userRole;
            }

            //buscar ids selecionados e carregar o model
            if (userRole == "CondoMember")
            {
                var selectedCompaniesIds = user.CompaniesDto.Select(c => c.Id).ToList();
                model.SelectedCompaniesIds = selectedCompaniesIds;

            }
            else
            {
                var selectedCompanyId = user.CompaniesDto.Select(c => c.Id).First();
                model.SelectedCompanyId = selectedCompanyId;
            }

            return model;
        }


        public async Task<AssignCompanyViewModel> LoadAssingCompanyViewModel(UserDto user)
        {
            //carregar lista
            var companiesList = await LoadCompaniesSelectList();

            var model = new AssignCompanyViewModel
            {
                Id = user.Id,
                CompaniesDto = user.CompaniesDto,
                Address = user.Address,
                FullName = user.FullName,
                FinancialAccountId = user.FinancialAccountId,
                AvailableCompanies = companiesList
            };

            //carregar companies do user 
            var userRole = await _apiCallService.GetAsync<string>($"api/Account/GetUserRole?email={user.Email}");

            //carregar userRole
            if (userRole != null)
            {
                model.UserRole = userRole;
            }

            //buscar ids selecionados e carregar o model
            if (userRole == "CondoMember")
            {
                var selectedCompaniesIds = user.CompaniesDto.Select(c => c.Id).ToList();
                model.SelectedCompaniesIds = selectedCompaniesIds;

            }
            else
            {
                var selectedCompanyId = user.CompaniesDto.Select(c => c.Id).First();
                model.SelectedCompanyId = selectedCompanyId;
            }

            return model;
        }


        [Authorize(Roles = "CondoManager")]
        public async Task<ActionResult<CondoManagerDashboardViewModel>> CondoManagerDashboard()
        {
            var email = User.Identity?.Name;

            var model = new CondoManagerDashboardViewModel();

            var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");
            model.CondoManager = user;

            var condominiums = await _apiCallService.GetAsync<List<CondominiumDto>>($"api/Condominiums/ByManager")
                               ?? new List<CondominiumDto>();

            model.Condominiums = condominiums;

            model.Occurrences.AddRange(
                condominiums.SelectMany(c => c.Occurrences ?? new List<OccurrenceDto>())
                            .Where(o => !o.IsResolved)
                            .OrderByDescending(o => o.DateAndTime)
                            .Take(5)
            );
            
            
            

            model.Payments.AddRange(
                condominiums.SelectMany(c => c.Payments ?? new List<PaymentDto>())
                            .Where(p => !p.IsPaid && p.DueDate >= DateTime.Now)
                            .OrderBy(p => p.DueDate)
                            .Take(5)
            );

            model.Meetings.AddRange(
                condominiums.SelectMany(c => c.Meetings ?? new List<MeetingDto>())
                            .Where(m => m.DateAndTime >= DateTime.Now)
                            .OrderBy(m => m.DateAndTime)
                            .Take(5)
            );


            model.Messages = await _apiCallService.GetAsync<List<MessageDto>>($"api/Message/Received/{email}")?? new List<MessageDto>();

            return View(model);
        }

        [Authorize(Roles = "CompanyAdmin")]
        public async Task<ActionResult<CompanyAdminDashboardViewModel>> CompanyAdminDashboard()
        {
            var email = User.Identity?.Name;
            var model = new CompanyAdminDashboardViewModel();

            try
            {
                
                var user = await _apiCallService.GetAsync<UserDto>($"api/Account/GetUserByEmail2?email={email}");
                var companies = await _apiCallService.GetAsync<List<CompanyDto>>($"api/Company/GetCompanyByUser?includeCondominiums=true");

                var company = companies.FirstOrDefault(); //um company admin só tem uma company

                model.CompanyAdmin = user;

                model.FinancialAccount = await _apiCallService.GetAsync<FinancialAccountDto>($"api/FinancialAccounts/{company.FinancialAccountId}");
                model.Payments = await _apiCallService.GetAsync<List<PaymentDto>>($"api/Payment/GetPaymentsByFinancialAccount?financialAccountId={model.FinancialAccount.Id}") ?? new List<PaymentDto>();
                model.Condominiums = company.CondominiumDtos?.ToList() ?? new List<CondominiumDto>();
                model.CondoManagers = await _apiCallService.GetAsync<List<UserDto>>($"api/Account/GetManagers") ?? new List<UserDto>();

                model.Messages = await _apiCallService.GetAsync<List<MessageDto>>($"api/Message/Received/{email}") ?? new List<MessageDto>();



                foreach (var condo in model.Condominiums)
                {
                    condo.ManagerUser = model.CondoManagers.FirstOrDefault(m => m.Id == condo.ManagerUserId);
                }

                return View(model);

            }
            catch (Exception ex)
            {

                return View(model);
            }

            

        }


    }
}




