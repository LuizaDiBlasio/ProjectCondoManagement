using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Vereyon.Web;
using ClassLibrary.DtoModels;
using System.IdentityModel.Tokens.Jwt;
using ClassLibrary;

namespace CondoManagementWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IFlashMessage _flashMessage;
        private readonly IConfiguration _configuration;
        private readonly IConverterHelper _converterHelper;
        private readonly CloudinaryService _cloudinaryService;
        private readonly HttpClient _httpClient;

        private readonly string baseUrl = "https://localhost:7001/"; //TODO : Mudar depois que publicar

        public AccountController(IFlashMessage flashMessage, IConfiguration configuration, HttpClient httpClient,
            IConverterHelper converterHelper, CloudinaryService cloudinaryService)
        {

            _flashMessage = flashMessage;
            _configuration = configuration;
            _httpClient = httpClient;
            _cloudinaryService = cloudinaryService;
            _converterHelper = converterHelper; 
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
        /// Attempts to get a JWT token for 'Employee' roles from an external API.
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
        /// Logs out the current user and redirects to the Home page.
        /// </summary>
        /// <returns>A redirection to the Home page.</returns>
        [Microsoft.AspNetCore.Authorization.Authorize] // todos os roles autenticados
        public async Task<IActionResult> Logout()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}api/UserApiAccountController/Logout");

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
        public async Task<IActionResult> Register(RegisterUserViewModel model) // registra o user
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
    }
}
