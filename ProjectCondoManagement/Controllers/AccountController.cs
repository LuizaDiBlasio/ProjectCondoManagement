using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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
        private readonly ISmsHelper _smsHelper;

        string webBaseAdress = "https://localhost:7081"; //TODO mudar quando publicar

        public AccountController(IUserHelper userHelper, HttpClient httpClient, IConfiguration configuration, IConverterHelper converterHelper,
                               IMailHelper mailHelper, DataContextCondos dataContextCondos, IJwtTokenService jwtTokenService, ICondoMemberRepository condoMemberRepository
                             , ISmsHelper smsHelper)
        {
            _userHelper = userHelper;
            _httpClient = httpClient;
            _configuration = configuration;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _dataContextCondos = dataContextCondos;
            _jwtTokenService = jwtTokenService;
            _condoMemberRepository = condoMemberRepository;
            _smsHelper = smsHelper;
       
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDtoModel)
        {
            //ver se user está ativo
            var user = await _userHelper.GetUserByEmailAsync(loginDtoModel.Username);

            if(user == null)
            {
                return NotFound(new { Message = "Login failed, user not found." });
            }

            if (user.IsActive == false)
            {
                return Unauthorized(new Response { Message = "User is not active in the system, please contact admin" });
            }

            var result = await _userHelper.LoginAsync(loginDtoModel); //fazer login (Este é o seu método que usa PasswordSignInAsync internamente)

            if (result.RequiresTwoFactor) //se login for bem sucedido
            {
                var token = await _userHelper.GenerateTwoFactorTokenAsync(user, "Phone");

                var response = await _smsHelper.SendSmsAsync("+351936752044", $"Your authentication code is: {token}");

                if (response.IsSuccess)
                {
                    return Ok(new TokenResponseModel
                    {
                        Token = null,
                        Expiration = null,
                        Requires2FA = true,
                    });
                }
                else
                {
                    return StatusCode(500, new { Message = "It was not possible to send SMS verification code" });
                }
            }
            else //seguir normalmente para ambiente de desenvolvimento //TODO todo esse else vai ser apagado antes de publicar 
            {
                // pede lista de roles do usuário (somente 1 item na lista)
                var roles = await _userHelper.GetRolesAsync(user);

                // verifica se a lista não está vazia e pega a primeira role.
                if (roles != null && roles.Any())
                {
                    var userRole = roles.First();

                    // gerar o token jwt
                    var tokenJwt = _jwtTokenService.GenerateToken(user, userRole);

                    var results = new TokenResponseModel()
                    {
                        Token = tokenJwt,
                        Expiration = DateTime.UtcNow.AddDays(15),
                        Requires2FA = false
                    };

                    return Ok(results);
                }


            }
            return Unauthorized(new { Message = "Login failed, credentials are not valid." });
        }



        [HttpPost("Verify2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto verify2FADto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userHelper.GetUserByEmailAsync(verify2FADto.Username);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Verifica o token 2FA
            var isValidToken = await _userHelper.VerifyTwoFactorTokenAsync(user, "Phone", verify2FADto.Token);

            if (isValidToken)
            {
                // Token válido - gera JWT
                var roles = await _userHelper.GetRolesAsync(user);

                if (roles != null && roles.Any())
                {
                    var userRole = roles.First();
                    var tokenJwt = _jwtTokenService.GenerateToken(user, userRole);

                    var results = new TokenResponseModel()
                    {
                        Token = tokenJwt,
                        Expiration = DateTime.UtcNow.AddDays(15),
                        Requires2FA = false
                    };

                    return Ok(results);
                }
            }

            return BadRequest(new { Message = "Invalid verification code" });
        }



        [HttpPost("GenerateForgotPasswordTokenAndEmail")]
        public async Task<IActionResult> GenerateForgotPasswordTokenAndEmail([FromBody] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                return StatusCode(404, new Response { Message = "User not found", IsSuccess = false });
            }

            string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user); //gerar o token

            // gera um link de confirmção para o email
            string tokenLink = $"{webBaseAdress}/Account/RecoverPassword?userId={user.Id}&token={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response response = _mailHelper.SendEmail(email, "Retrieve your password", $"<h1>Retrieve your password</h1>" +
           $"<br><br><a href = \"{tokenLink}\">Click here to reset your password</a>"); //Contruir email e enviá-lo com o link

            if (response.IsSuccess) //se conseguiu enviar o email
            {
                return StatusCode(200, new {  Message = "A link to retrieve password has been sent to your email" });
            }

            //se não conseguiu enviar email:
            return StatusCode(500, new  { Message = "Unable to retrieve password, please contact admin" });

        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SysAdmin")]
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



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SysAdmin")]
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



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {

            var user = await _userHelper.GetUserByEmailAsync(changePasswordDto.Email); //verificar user

            if (user != null)
            {
                var result = await _userHelper.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword); //muda password

                if (result.Succeeded)
                {
                    return StatusCode(200, new Response() { IsSuccess = true, Message = "Your password has been changed."});
                }
                else
                {
                    return StatusCode(400, new Response() { IsSuccess = false, Message = "Unable to change password" });
                }
            }
            else //se for nulo
            {
                return StatusCode(404, new Response() { IsSuccess = false, Message = "Unable to change password" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpPost("GetUserByEmail")]

        public async Task<IActionResult> GetUserByEmail([FromBody] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {    
                return NotFound(null);
            }

            var userDto = _converterHelper.ToUserDto(user);

            return Ok(userDto);    
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpPost("EditProfile")]

        public async Task<IActionResult> EditProfile([FromBody] UserDto userDto)
        {
            var user = await _userHelper.GetUserByEmailAsync(userDto.Email);
            
            if (user == null)
            {
                return NotFound(null);  
            }

            var response = await _userHelper.UpdateUserAsync(user);

            if(response.Succeeded)
            {
                var editedUser = await _userHelper.GetUserByEmailAsync(userDto.Email);

                var editedUserDto = _converterHelper.ToUserDto(editedUser);

                return Ok(editedUserDto);
            }

            return StatusCode(500, new { error = "An internal server error occurred." });
        }

    }
}


