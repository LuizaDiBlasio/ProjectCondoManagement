using ClassLibrary;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.FinancesDb;
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
        private readonly IApiCallService _apiCallService;
        private readonly HttpClient _httpClient;

        private readonly string baseUrl = "https://localhost:7001/"; //TODO : Mudar depois que publicar

        public AccountController(IFlashMessage flashMessage, IConfiguration configuration, HttpClient httpClient,
            IConverterHelper converterHelper, CloudinaryService cloudinaryService,IApiCallService apiCallService)
        {

            _flashMessage = flashMessage;
            _configuration = configuration;
            _httpClient = httpClient;
            _cloudinaryService = cloudinaryService;
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
        }

        /// <summary>
        /// Displays the login view. If the user is already authenticated, redirects to the Home page.
        /// </summary>
        /// <returns>The login view or a redirection to the Home page.</returns>
        //Get do login 
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) //caso usuário esteja autenticado
            {
                return RedirectToAction("Index", "Home"); //mandar para a view Index que possui o controller Home
            }

            return View(); //se login não funcionar, permanece na View 
        }

        /// <summary>
        /// Displays the login view. If the user is already authenticated, redirects to the Home page.
        /// </summary>
        /// <returns>The login view or a redirection to the Home page.</returns>
        //Get do Change login 
        public IActionResult ChangeLogin()
        {

            return View();
        }


        /// <summary>
        /// Handles the login POST request, authenticates the user, and redirects based on role.
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
                    var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                    //armazenar token na sessão para futuras requisições da api
                    HttpContext.Session.SetString("JwtToken", tokenResponse.Token);

                    // ClaimsPrincipal com base nas claims do token JWT
                    var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // login no  WebApp, criando um cookie de autenticação
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    _flashMessage.Confirmation("Login successful!");
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
        [Microsoft.AspNetCore.Authorization.Authorize]
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

        public IActionResult Register() //só mostra a view do Register
        {
            //criar modelo com as opções da combobox
            var selectList = new List<SelectListItem>
            {
                new SelectListItem{Value = "0", Text = "Select a role..."},
                new SelectListItem{Value = "CondoManager", Text = "CondoManager"},
                new SelectListItem{Value = "CondoMember", Text = "CondoMember"},
                new SelectListItem{Value = "Admin", Text = "Admin"},
            };

            var model = new RegisterUserViewModel();

            model.AvailableRoles = selectList;

            return View(model);
        }

        /// <summary>
        /// Processes the registration of a new user.
        /// Only users with the 'Admin' role can access this functionality.
        /// </summary>
        /// <param name="model">The model containing the data of the new user to be registered.</param>
        /// <returns>An action result indicating success or failure of the registration.</returns>

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> RequestRegister(RegisterUserViewModel model) // registra o user
        {
            if (ModelState.IsValid) //ver se modelo é válido
            {

                if (model.SelectedRole == "0") // Se a opção "Select a role..." foi selecionada
                {
                    ModelState.AddModelError("SelectedRole", "You must select a valid role."); // Adiciona um erro específico para o campo SelectedRole

                    return View(model); // Retorna a View com o erro
                }

                //Blobar imagem
                Guid imageId = Guid.Empty; // identificador da imagem no blob (ainda não identificada)

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var url = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                    model.ImageUrl = url;
                }


                //converter para dto
                var registerDto = _converterHelper.ToRegisterDto(model);

                //Serializar
                var jsonContent = new StringContent(
                JsonSerializer.Serialize(registerDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), //converte para camelCase em json
                   Encoding.UTF8,
                   "application/json" //diz que o body vai ter dados em json
               );

                // Fazer a requisição HTTP POST para a API

                var response = await _httpClient.PostAsync($"{baseUrl}api/AccountController/Register", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    _flashMessage.Confirmation("Confirmation instructions have been sent to user's email");

                    return View(model);
                }
                else // Login falhou na API
                {
                    var errorResponseContent = await response.Content.ReadAsStringAsync();
                    string errorMessage = "An unexpected error occurred, user cannot be registered";

                    try
                    {
                        // pegar mensagens de erro da api
                        var apiError = JsonSerializer.Deserialize<Dictionary<string, string>>(errorResponseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        if (apiError != null && apiError.ContainsKey("message"))
                        {
                            errorMessage = apiError["message"];
                        }
                    }
                    catch (JsonException)
                    {
                        _flashMessage.Danger("An unexpected error occurred, user cannot be registered");
                    }

                    this.ModelState.AddModelError(string.Empty, errorMessage);
                    _flashMessage.Danger(errorMessage); // Exibe a mensagem de erro vinda da API ou a padrão
                    return View(model); // Retorna a View de login com o modelo e a mensagem de erro
                }
            }
            // Se o result.Succeeded for false (login falhou )
            this.ModelState.AddModelError(string.Empty, "An unexpected error occurred, user cannot be registered");
            _flashMessage.Danger("An unexpected error occurred, user cannot be registered");

            return View(model);

        }

        
        public async Task<IActionResult> CompanyAdminDashboard()
        {
            var model = new CompanyAdminDashboardViewModel
            {
                Fees = await GetFeesAsync()
            };

            return View(model);
        }

        public async Task<IEnumerable<Fee>> GetFeesAsync()
        {
            var fees = await _apiCallService.GetAsync<IEnumerable<Fee>>("api/Finances/GetFees");

            return fees;
        }

        // <summary>
        /// Displays the "Not Authorized" view when a user tries to access a restricted area.
        /// </summary>
        /// <returns>The "Not Authorized" view.</returns>
        public IActionResult NotAuthorized()
        {
            return View();
        }

    }
}

//Explicação do fluxo do Login com jwt (apagar depois):

//O login request do frontend faz uma requisição HTTP com as informações de login para a API. A API checa essas informações,
//caso elas sejam válidas, gera um token JWT Bearer e devolve isso para o frontend na sua resposta. O frontend, que é uma aplicação ASP.NET Core Web,
//recebe o token, desserializa-o para extrair as claims (informações do usuário) e usa essas claims para criar um cookie de autenticação. Este cookie
//será usado para manter o usuário autenticado na sessão do browser, permitindo o acesso às páginas protegidas do Web App. O Controller Account do cliente
//também se encarrega de armazenar o token Jwt dentro de novos para que seja possível acessar aos dados via api. O Program.cs do frontend configura
//a autenticação por cookies, enquanto o Program.cs do backend (API) configura o esquema de autenticação para validar os tokens JWT que são enviados em cada
//requisição feita pelo  Web App (ou por qualquer outro cliente da API) para ele. O User Identity é usado na validação de informações do user na API,
//mas deixa de ser utilizado para criação de token e armazenamento de cookie como no projeto passado

//A cada requisição:
//para cada requisição é necessário adicionar o token ao cabeçalho do jsno que será enviado na requisição http

