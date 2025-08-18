using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    [Area("Api")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly DataContextCondos _dataContextCondos;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly DataContextUsers _dataContextUsers;
        string webBaseAdress = "https://localhost:7081"; //TODO mudar quando publicar


        public AccountController(IUserHelper userHelper, HttpClient httpClient, IConfiguration configuration, IConverterHelper converterHelper,
                                        IMailHelper mailHelper, DataContextCondos dataContextCondos, IJwtTokenService jwtTokenService, DataContextUsers dataContextUsers)
        {
            _userHelper = userHelper;
            _httpClient = httpClient;
            _configuration = configuration;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _dataContextCondos = dataContextCondos;
            _jwtTokenService = jwtTokenService;
            _dataContextUsers = dataContextUsers;
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

                if (user != null)
                {
                    // pede lista de roles do usuário (somente 1 item na lista)
                    var roles = await _userHelper.GetRolesAsync(user);

                    // verifica se a lista não está vazia e pega a primeira role.
                    if (roles != null && roles.Any())
                    {
                        var userRole = roles.First();

                        // string da role para gerar o token
                        var tokenString = _jwtTokenService.GenerateToken(user, userRole);

                        var results = new
                        {
                            token = tokenString,
                            expiration = DateTime.UtcNow.AddDays(15)
                        };

                        return Ok(results);

                    }
                    return Unauthorized(new { Message = "Login failed, credentials are not valid." });

                }
                return NotFound(new { Message = "Login failed, user not found." });
            }
            return Unauthorized(new { Message = "Login failed." });
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("AssociateUser")]
        public async Task<IActionResult> AssociateUser([FromBody] RegisterUserDto registerDtoModel)
        {
            var user = await _userHelper.CreateUser(registerDtoModel);

            if (user == null)
            {
                return Ok(new Response { IsSuccess = true, Message = "User registered, a confirmation email has been sent" });
            }

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //gerar o token

            // gera um link de confirmção para o email
            string tokenLink = $"{webBaseAdress}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
           $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password</a>"); //Contruir email e enviá-lo com o link

            if (response.IsSuccess) //se conseguiu enviar o email
            {
                return Ok(new Response { IsSuccess = true, Message = "User registered, a confirmation email has been sent" });
            }

            //se não conseguiu enviar email:
            return StatusCode(500, new Response { IsSuccess = false, Message = "Internal server error: User not registered" });
        }




        [Microsoft.AspNetCore.Mvc.HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDtoModel)
        {
      
            var user = await _userHelper.CreateUser(registerDtoModel);
            if (user == null) 
            {
               return StatusCode(500, new { Message = "Internal server error: User not registered" });
            }

            var isCondoMember = await _userHelper.IsUserInRoleAsync(user, "CondoMember");


                if (isCondoMember) // caso o user seja um condomember, criar condomember programaticamente
                {
                    var condoMember = new CondoMember()
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        ImageUrl = user.ImageUrl,
                        BirthDate = user.BirthDate,
                    };

                    await _condoMemberRepository.CreateAsync(condoMember, _dataContextCondos);
                }

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //gerar o token

            string tokenLink = $"{webBaseAdress}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
           $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password</a>"); //Contruir email e enviá-lo com o link

            if (response.IsSuccess) //se conseguiu enviar o email
            {
                return StatusCode(200, new { Message = "User registered, a confirmation email has been sent" });
            }

            //se não conseguiu enviar email:
            return StatusCode(500, new { Message = "User couldn't be logged" });


        }

    }
}


