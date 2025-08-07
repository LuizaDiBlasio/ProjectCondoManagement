using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using static System.Net.WebRequestMethods;

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
        private readonly ICondoMemberRepository _condoMemberRepository;
        private readonly DataContextCondos _dataContextCondos;
        private readonly IJwtTokenService _jwtTokenService;

        string webBaseAdress = "https://localhost:7081"; //TODO mudar quando publicar

        public AccountController(IUserHelper userHelper, HttpClient httpClient, IConfiguration configuration, IConverterHelper converterHelper,
                               IMailHelper mailHelper, DataContextCondos dataContextCondos, IJwtTokenService jwtTokenService, ICondoMemberRepository condoMemberRepository)
        {
            _userHelper = userHelper;
            _httpClient = httpClient;
            _configuration = configuration;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _dataContextCondos = dataContextCondos;
            _jwtTokenService = jwtTokenService;
            _condoMemberRepository = condoMemberRepository;

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

                        // gerar o token jwt
                        var tokenJwt = _jwtTokenService.GenerateToken(user, userRole);

                        var results = new
                        {
                            JwtToken = tokenJwt,
                            JwtExpiration = DateTime.UtcNow.AddDays(15)
                        };

                        // gerar o token 2FA
                        var token2FA = _userHelper.GenerateTwoFactorTokenAsync(user, token) //Estrnho

                        return Ok(results);

                    }
                    return Unauthorized(new { Message = "Login failed, credentials are not valid." });

                }
                return NotFound(new { Message = "Login failed, user not found." });
            }
            return Unauthorized(new { Message = "Login failed." });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [Microsoft.AspNetCore.Mvc.HttpPost("AssociateUser")]
        public async Task<IActionResult> AssociateUser([FromBody] RegisterUserDto registerDtoModel)
        {
            var user = await _userHelper.CreateUser(registerDtoModel);

            if (user == null)
            {
                return StatusCode(500, new { Message = "Internal server error: User not registered" });
            }

            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //gerar o token

            // gera um link de confirmção para o email
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [Microsoft.AspNetCore.Mvc.HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDtoModel)
        {
            var user = await _userHelper.GetUserByEmailAsync(registerDtoModel.Email); //buscar user  

            if (user != null) 
            {
                return StatusCode(409, new Response { Message = "User already exists, try registering wih new credentials", IsSuccess = false });
            }
            else //criar user caso não exista
            {
                var newUser = await _userHelper.CreateUser(registerDtoModel);

                if (newUser == null)//se retorna null, CreateUser foi mal sucedido 
                {
                    return StatusCode(500, new { Message = "Internal server error: User not registered" });
                }

                //Adicionar roles ao user
                switch (registerDtoModel.SelectedRole)
                {
                    case "CondoMember":
                        await _userHelper.AddUserToRoleAsync(newUser, "CondoMember");
                        break;
                    case "CondoManager":
                        await _userHelper.AddUserToRoleAsync(newUser, "CondoManager");
                        break;
                    case "Admin":
                        await _userHelper.AddUserToRoleAsync(newUser, "Admin");
                        break;
                }

                var isCondoMember = await _userHelper.IsUserInRoleAsync(newUser, "CondoMember");

                if (isCondoMember) // caso o user seja um condomember, criar condomember programaticamente
                {
                    var condoMember = new CondoMember()
                    {
                        FullName = newUser.FullName,
                        Email = newUser.Email,
                        Address = newUser.Address,
                        PhoneNumber = newUser.PhoneNumber,
                        ImageUrl = newUser.ImageUrl,
                        BirthDate = newUser.BirthDate
                    };

                    await _condoMemberRepository.CreateAsync(condoMember, _dataContextCondos);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(newUser); //gerar o token

                // gera um link de confirmção para o email
                string tokenLink = $"{webBaseAdress}/Account/ResetPassword?userId={newUser.Id}&tokenEmail={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

                Response response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
               $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password. </a>"); //Contruir email e enviá-lo com o link 

                if (response.IsSuccess) //se conseguiu enviar o email
                {
                    return StatusCode(200, new Response { Message = "User registered, a confirmation email has been sent", IsSuccess = true });
                }

                //se não conseguiu enviar email:
                return StatusCode(500, new Response { Message = "User couldn't be logged", IsSuccess = false });
            }   
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("GenerateResetPasswordToken")]
        public async Task<IActionResult> GenerateResetPasswordToken([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userHelper.GetUserByIdAsync(resetPasswordDto.UserId); //verificar user

            if (user == null)
            {
                return StatusCode(404, new Response { Message = "User not found", IsSuccess = false });
            }

            var response = await _userHelper.ConfirmEmailAsync(user, resetPasswordDto.Token); //resposta do email, ver se user e token dão match

            if (!response.Succeeded)
            {
                return StatusCode(404, new Response { Message = "User not found", IsSuccess = false });
            }

            //gerar token
            var passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            return StatusCode(200, new Response { Token = passwordToken, IsSuccess = true });
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userHelper.GetUserByIdAsync(resetPasswordDto.UserId); //verificar user

            if (user == null)
            {
                return StatusCode(404, new Response { Message = "User not found", IsSuccess = false });
            }

            var resetPassword = await _userHelper.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

            if (resetPassword.Succeeded)
            {
                return StatusCode(200, new Response { Message = "Password reset successfully, you can login now", IsSuccess = true });
            }
            else
            {
                return StatusCode(400, new Response { Message = "An unexpected error occurred while resetting password, please try again", IsSuccess = false });
            }   
        }
    }
}


