using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Helpers;
using System.Text.Json;
using System.Text;
using ClassLibrary;

namespace ProjectCondoManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, HttpClient httpClient, IConfiguration configuration, IConverterHelper converterHelper,
                                        IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _httpClient = httpClient;
            _configuration = configuration;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDtoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userHelper.LoginAsync(loginDtoModel); //fazer login (Este é o seu método que usa PasswordSignInAsync internamente)

            if (result.Succeeded) //se login for bem sucedido
            {
                var user = await _userHelper.GetUserByEmailAsync(loginDtoModel.Username);

                // Verificação de usuário e roles para a API e redirecionamento (existente) 
                if (user == null)
                {
                    return StatusCode(500, new { Message = "Internal server error: User not found after successful authentication." });
                }

                //TODO vamos usar Jwt token?
                return Ok(new { Message = "Login successful", /* , Token = "jwt_aqui" */ });

            }
            return Unauthorized(new { Message = "Login failed, credentials are not valid." });
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return Ok();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDtoModel)
        {
            var user = await _userHelper.GetUserByEmailAsync(registerDtoModel.Email); //buscar user  

            if (user == null) // caso user não exista, registrá-lo
            {
                user = new User
                {
                    FullName = registerDtoModel.FullName,
                    Email = registerDtoModel.Email,
                    UserName = registerDtoModel.Email,
                    Address = registerDtoModel.Address,
                    PhoneNumber = registerDtoModel.PhoneNumber,
                    ImageUrl = registerDtoModel.ImageUrl,
                    BirthDate = registerDtoModel.BirthDate,
                    CompanyId = registerDtoModel.CompanyId,
                };

                var result = await _userHelper.AddUserAsync(user, "123456"); //add user depois de criado

                if (result != IdentityResult.Success) // caso não consiga criar user
                {
                    return StatusCode(500, new { Message = "Internal server error: User not registered" });
                }

                //Adicionar roles ao user
                switch (registerDtoModel.SelectedRole)
                {
                    case "Student":
                        await _userHelper.AddUserToRoleAsync(user, "CondoMemeber");
                        break;
                    case "Employee":
                        await _userHelper.AddUserToRoleAsync(user, "CondoManager");
                        break;
                    case "Admin":
                        await _userHelper.AddUserToRoleAsync(user, "Admin");
                        break;
                }

                var isCondoMember = await _userHelper.IsUserInRoleAsync(user, "CondoMember");

                if (isCondoMember) // caso o user seja um condomember, criar condomember por requisição http
                {
                    //Converter
                    var condoMemberDtoModel = _converterHelper.ToCondoMemberDto(user);

                    //Serializar 

                    var jsonContent = new StringContent(
                   JsonSerializer.Serialize(condoMemberDtoModel, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), //converte para camelCase em json
                   Encoding.UTF8,
                   "application/json" //diz que o body vai ter dados em json
                    );

                    // Fazer a requisição HTTP POST para a CondoAPI
                    string userApiBaseUrl = _configuration["ApiUrls:CondoApi"];

                    var responseCondo = await _httpClient.PostAsync($"{userApiBaseUrl}/api/UserApiCondoMemberController/Create", jsonContent);

                    if (!responseCondo.IsSuccessStatusCode)
                    {
                        var errorResponseContent = await responseCondo.Content.ReadAsStringAsync();
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

                        }

                        this.ModelState.AddModelError(string.Empty, errorMessage);

                    }
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //gerar o token

                // gera um link de confirmção para o email
                string tokenLink = Url.Action("ResetPassword", "Account", new  //Link gerado na Action ConfirmEmail dentro do AccountController, ela recebe 2 parametros (userId e token)
                {
                    userId = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme); //utiliza o protocolo Http para passar dados de uma action para a outra

                Response response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
               $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password</a>"); //Contruir email e enviá-lo com o link 

                if (response.IsSuccess) //se conseguiu enviar o email
                {
                    return StatusCode(200, new { Message = "User registered, a confirmation email has been sent" });
                }

                //se não conseguiu enviar email:
                return StatusCode(500, new { Message = "User couldn't be logged" });


            }
            else
            {
                return StatusCode(500, new { Message = "User already exists, try registering wih new credentials" });
            }
        }
    }
}
