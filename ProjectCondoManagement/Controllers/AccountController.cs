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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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


                if (result != IdentityResult.Success) // caso não consiga criar user
                {
                    return StatusCode(500, new { Message = "Internal server error: User not registered" });
                }

                //Adicionar roles ao user
                switch (registerDtoModel.SelectedRole)
                {
                    case "CondoMember":
                        await _userHelper.AddUserToRoleAsync(user, "CondoMember");
                        break;
                    case "CondoManager":
                        await _userHelper.AddUserToRoleAsync(user, "CondoManager");
                        break;
                    case "Admin":
                        await _userHelper.AddUserToRoleAsync(user, "Admin");
                        break;
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
                    return StatusCode(200, new Response { Message = "User registered, a confirmation email has been sent", IsSuccess = true });
                }

                //se não conseguiu enviar email:
                return StatusCode(500, new Response { Message = "User couldn't be logged", IsSuccess = false });
            }
            else
            {
                return StatusCode(409, new Response { Message = "User already exists, try registering wih new credentials", IsSuccess = false });
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userHelper.GetUserByIdAsync(resetPasswordDto.UserId); //verificar user

            if (user == null)
            {
                return StatusCode(404, new Response { Message = "User not found", IsSuccess = false });
            }

            var result = await _userHelper.ConfirmEmailAsync(user, resetPasswordDto.Token); //resposta do email, ver se user e token dão match

            if (!result.Succeeded)
            {
                return StatusCode(404, new Response { Message = "User not found", IsSuccess = false });
            }

            //gerar token

            var passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            var responseResult = new
            {
                Username = user.UserName,
                Token = passwordToken,
            };

            var response = new Response
            {
                IsSuccess = true,
                Message = "Access permission granted",
                Results = responseResult
            };

            return Ok(response);
        }
    }
}


