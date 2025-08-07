using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        private readonly string baseUrl = "https://localhost:7001/"; //TODO : Mudar depois que publicar

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

            if (ModelState.IsValid) // se modelo enviado passar na validação
            {

                var loginDto = _converterHelper.ToLoginDto(model);

                // Serializar model para JSON
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(loginDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), //converte para camelCase em json
                    Encoding.UTF8,
                    "application/json" //diz que o body vai ter dados em json
                );

                // Fazer a requisição HTTP POST para a UserAPI

                var response = await _httpClient.PostAsync($"{baseUrl}api/Account/Login", jsonContent);

                if (response.IsSuccessStatusCode)
                {

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponseModel>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    //sessão com token
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(tokenResponse.JwtToken);

                    //armazenar token na sessão para futuras requisições da api
                    HttpContext.Session.SetString("JwtToken", tokenResponse.JwtToken);

                    // ClaimsPrincipal com base nas claims do token JWT
                    var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // login no  WebApp, criando um cookie de autenticação
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return RedirectToAction("Index", "Home");


                }
                else // Login falhou na API
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                    return View("Login", model);
                }
            }
            // Se o result.Succeeded for false (login falhou )
            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View("Login", model);
        }



        /// <summary>
        /// Cleans Session, remove cookies and redirects to the Home page. 
        /// </summary>
        /// <returns>A redirection to the Home page.</returns>
        [Authorize(Roles = "Admin, CondoManager, CondoMember")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Register() //só mostra a view do Register
        {
            //criar modelo com as opções da combobox
            var selectList = new List<SelectListItem>
            {
                new SelectListItem{Value = "0", Text = "Select a role..."},
                new SelectListItem{Value = "CondoManager", Text = "Condo manager"},
                new SelectListItem{Value = "CondoMember", Text = "Condo member"},
                new SelectListItem{Value = "Admin", Text = "Admin"},
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
        [Authorize(Roles = "Admin")]
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

            return View("Register",model);
        }


        /// <summary>
        /// Displays the view for resetting the user's password after email confirmation.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>The password reset view or a "User Not Found" view if parameters are invalid.</returns>
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
                Password =String.Empty  //ainda não foi colocada a senha
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
                        Password = String.Empty //ainda não foi colocada a senha
                    };


                    ModelState.Remove("Token"); //limpa o ModelState para evitar o uso do token antigo de confirmação de email

                    return View( model2);
                }

                return View("Error");
            }
            catch
            {
                return View("Error");
            }
                
        }

        /// <summary>
        /// Processes the user's password reset request and requests for SMS confirmation token.
        /// </summary>
        /// <param name="model">The model containing the username, reset token, and new password.</param>
        /// <returns>The password reset view with a success or error message.</returns>
        [HttpPost]
        public async Task<IActionResult> SendResetPassword(ResetPasswordViewModel model) //recebo modelo preechido com dados para reset da password
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

//Explicação do fluxo do Login com jwt (apagar depois):

//O login apiCall do frontend faz uma requisição HTTP com as informações de login para a API. A API checa essas informações,
//caso elas sejam válidas, gera um token JWT Bearer e devolve isso para o frontend na sua resposta. O frontend, que é uma aplicação ASP.NET Core Web,
//recebe o token, desserializa-o para extrair as claims (informações do usuário) e usa essas claims para criar um cookie de autenticação. Este cookie
//será usado para manter o usuário autenticado na sessão do browser, permitindo o acesso às páginas protegidas do Web App. O Controller Account do cliente
//também se encarrega de armazenar o token Jwt dentro de novos para que seja possível acessar aos dados via api. O Program.cs do frontend configura
//a autenticação por cookies, enquanto o Program.cs do backend (API) configura o esquema de autenticação para validar os tokens JWT que são enviados em cada
//requisição feita pelo  Web App (ou por qualquer outro cliente da API) para ele. O User Identity é usado na validação de informações do user na API,
//mas deixa de ser utilizado para criação de token e armazenamento de cookie como no projeto passado

//A cada requisição:
//para cada requisição é necessário adicionar o token ao cabeçalho do json que será enviado na requisição http fazemos isso por meio do serviços de userApiCall
//e da interface apiCall para as demais entidades


