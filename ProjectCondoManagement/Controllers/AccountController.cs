using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
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
        private readonly DataContextFinances _dataContextFinances;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ISmsHelper _smsHelper;
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly IWebHostEnvironment _env;






        public AccountController(IUserHelper userHelper, HttpClient httpClient, IConfiguration configuration, IConverterHelper converterHelper,
                               IMailHelper mailHelper, DataContextCondos dataContextCondos, DataContextFinances dataContextFinances, IJwtTokenService jwtTokenService,
                               ICondoMemberRepository condoMemberRepository , ISmsHelper smsHelper, IFinancialAccountRepository financialAccountRepository, IWebHostEnvironment env)
        {
            _userHelper = userHelper;
            _httpClient = httpClient;
            _configuration = configuration;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _dataContextCondos = dataContextCondos;
            _dataContextFinances = dataContextFinances;
            _jwtTokenService = jwtTokenService;
            _condoMemberRepository = condoMemberRepository;
            _smsHelper = smsHelper;
            _financialAccountRepository = financialAccountRepository;
            _env = env;
        }


        /// <summary>
        /// Authenticates a user based on their login credentials.
        /// If the user's account requires two-factor authentication (2FA), a verification token is sent via SMS.
        /// </summary>
        /// <param name="loginDtoModel">The login credentials, including username and password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a JWT token and expiration if login is successful and 2FA is not required,
        /// or a response indicating that 2FA is required and the token was sent.
        /// </returns>
        [Microsoft.AspNetCore.Mvc.HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDtoModel)
        {
            //ver se user está ativo
            var user = await _userHelper.GetUserByEmailAsync(loginDtoModel.Username);

            if (user == null)
            {
                return NotFound(new { Message = "Login failed, user not found." });
            }

            if (user.IsActive == false)
            {
                return Unauthorized(new Response<object> { Message = "User is not active in the system, please contact admin" });
            }

            var result = await _userHelper.LoginAsync(loginDtoModel); //fazer login 



            if (result.RequiresTwoFactor) //se login for bem sucedido
            {
                //TODO tirar _____________________________________________________________
                if (_env.IsDevelopment())
                {
                    // Gera token JWT direto
                    var roles = await _userHelper.GetRolesAsync(user);
                    var userRole = roles.FirstOrDefault() ?? "User";
                    var tokenJwt = _jwtTokenService.GenerateToken(user, userRole);

                    return Ok(new Response<object>
                    {
                        Token = tokenJwt,
                        Expiration = DateTime.UtcNow.AddDays(15),
                        Requires2FA = false,
                        Role = userRole,
                        IsSuccess = true,
                        Message = "2FA skipped in development"
                    });
                }
                //_______________________________________________________________________

                var token = await _userHelper.GenerateTwoFactorTokenAsync(user, "Phone");
                try
                {

                    var response = await _smsHelper.SendSmsAsync("+351936752044", $"Your authentication code is: {token}");
                    if (response.IsSuccess)
                    {
                        return Ok(new Response<object>
                        {
                            Token = null,
                            Expiration = null,
                            Requires2FA = true,
                            Role = null, // Não tem o role ainda aqui, precisa do 2FA.
                            IsSuccess = true
                        });
                    }
                    else
                    {
                        return StatusCode(500, new { Message = "It was not possible to send SMS verification code" });
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }


            }
            else
            {
                //TODO tirar isso   // Gera token JWT direto
                    var roles = await _userHelper.GetRolesAsync(user);
                    var userRole = roles.FirstOrDefault() ?? "User";
                    var tokenJwt = _jwtTokenService.GenerateToken(user, userRole);

                    return Ok(new Response<object>
                    {
                        Token = tokenJwt,
                        Expiration = DateTime.UtcNow.AddDays(15),
                        Requires2FA = false,
                        Role = userRole,
                        IsSuccess = true,
                        Message = "2FA skipped in development"
                    });

                //TODO descomentar
                //return StatusCode(500, new { Message = "Wrong credentials, please try again" });
            }
        }



       


        /// <summary>
        /// Verifies a user-provided two-factor authentication (2FA) token.
        /// If the token is valid, a JWT token is generated and returned to the client.
        /// </summary>
        /// <param name="verify2FADto">The user's username and the 2FA token to be verified.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a JWT token and expiration if the 2FA token is valid,
        /// otherwise, a BadRequest response with an error message.
        /// </returns>
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

                    var results = new Response<object>()
                    {
                        Token = tokenJwt,
                        Expiration = DateTime.UtcNow.AddDays(15),
                        Requires2FA = false,
                        Role = userRole,
                        IsSuccess = true,
                        Message = "Two-factor authentication successful."
                    };

                    return Ok(results);
                }
            }

            return BadRequest(new { Message = "Invalid verification code" });
        }


        /// <summary>
        /// Generates a password reset token for a user and sends a recovery link to their email.
        /// </summary>
        /// <param name="email">The email of the user requesting a password reset.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating if the email was sent successfully or if an error occurred.
        /// </returns>
        [HttpPost("GenerateForgotPasswordTokenAndEmail")]
        public async Task<IActionResult> GenerateForgotPasswordTokenAndEmail([FromBody] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                return StatusCode(404, new Response<object> { Message = "User not found", IsSuccess = false });
            }

            string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user); //gerar o token

            // gera um link de confirmção para o email
            string tokenLink = $"{_configuration["WebAppSettings:BaseUrl"]}/Account/RecoverPassword?userId={user.Id}&token={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response<object> response = _mailHelper.SendEmail(email, "Retrieve your password", $"<h1>Retrieve your password</h1>" +
           $"<br><br><a href = \"{tokenLink}\">Click here to reset your password</a>"); //Contruir email e enviá-lo com o link

            if (response.IsSuccess) //se conseguiu enviar o email
            {
                return StatusCode(200, new { Message = "A link to retrieve password has been sent to your email" });
            }

            //se não conseguiu enviar email:
            return StatusCode(500, new { Message = "Unable to retrieve password, please contact admin" });

        }


        /// <summary>
        /// Creates a new user account and sends a confirmation email. Method used to associate user to condoMember
        /// This endpoint is intended for use by a system administrator.
        /// </summary>
        /// <param name="registerDtoModel">The registration details for the new user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a success message if the user is registered and the email is sent,
        /// otherwise, an error response.
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            string tokenLink = $"{_configuration["WebAppSettings:BaseUrl"]}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response<object> response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
           $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password</a>"); //Contruir email e enviá-lo com o link

            if (response.IsSuccess) //se conseguiu enviar o email
            {
                return StatusCode(200, new Response<object> { Message = "User registered, a confirmation email has been sent", IsSuccess = true});
            }

            //se não conseguiu enviar email:
            return StatusCode(500, new Response<object> { IsSuccess = false, Message = "Internal server error: User not registered" });
        }


        /// <summary>
        /// Creates a new user account and sends a confirmation email.
        /// This endpoint is intended for use by a system administrator.
        /// </summary>
        /// <param name="registerDtoModel">The registration details for the new user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a success message if the user is registered and the email is sent,
        /// otherwise, an error response.
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SysAdmin")]
        [Microsoft.AspNetCore.Mvc.HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDtoModel)
        {
            var user = await _userHelper.GetUserByEmailAsync(registerDtoModel.Email); //buscar user  

            if (user != null)
            {
                return StatusCode(409, new Response<object> { Message = "User already exists, try registering wih new credentials", IsSuccess = false });
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
                string tokenLink = $"{_configuration["WebAppSettings:BaseUrl"]}/Account/ResetPassword?userId={newUser.Id}&tokenEmail={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

                Response<object> response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
               $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password. </a>"); //Contruir email e enviá-lo com o link 

                if (response.IsSuccess) //se conseguiu enviar o email
                {
                    return StatusCode(200, new Response<object> { Message = "User registered, a confirmation email has been sent", IsSuccess = true });
                }

                //se não conseguiu enviar email:
                return StatusCode(500, new Response<object> { Message = "User couldn't be logged", IsSuccess = false });
            }   
        }


        /// <summary>
        /// Verifies an email confirmation token and then generates a password reset token.
        /// This is a step in the password recovery process.
        /// </summary>
        /// <param name="resetPasswordDto">The user's ID and the email confirmation token.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a password reset token if the email token is valid,
        /// otherwise, a 404 Not Found response.
        /// </returns>
        [Microsoft.AspNetCore.Mvc.HttpPost("GenerateResetPasswordToken")]
        public async Task<IActionResult> GenerateResetPasswordToken([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userHelper.GetUserByIdAsync(resetPasswordDto.UserId); //verificar user

            if (user == null)
            {
                return StatusCode(404, new Response<object> { Message = "User not found", IsSuccess = false });
            }

            var response = await _userHelper.ConfirmEmailAsync(user, resetPasswordDto.Token); //resposta do email, ver se user e token dão match

            if (!response.Succeeded)
            {
                return StatusCode(404, new Response<object> { Message = "User not found", IsSuccess = false });
            }

            //gerar token
            var passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            return StatusCode(200, new Response<object> { Token = passwordToken, IsSuccess = true });
        }


        /// <summary>
        /// Resets a user's password using a valid password reset token.
        /// </summary>
        /// <param name="resetPasswordDto">The user's ID, the password reset token, and the new password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a success message if the password was reset successfully,
        /// otherwise, a 400 Bad Request response.
        /// </returns>
        [Microsoft.AspNetCore.Mvc.HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userHelper.GetUserByIdAsync(resetPasswordDto.UserId); //verificar user

            if (user == null)
            {
                return StatusCode(404, new Response<object> { Message = "User not found", IsSuccess = false });
            }

            var resetPassword = await _userHelper.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

            if (resetPassword.Succeeded)
            {
                return StatusCode(200, new Response<object> { Message = "Password reset successfully, you can login now", IsSuccess = true });
            }
            else
            {
                return StatusCode(400, new Response<object> { Message = "An unexpected error occurred while resetting password, please try again", IsSuccess = false });
            }   
        }


        /// <summary>
        /// Allows a logged-in user to change their password by providing their old and new passwords.
        /// </summary>
        /// <param name="changePasswordDto">The user's email, old password, and new password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with a success message if the password was changed,
        /// otherwise, a 400 Bad Request or 404 Not Found response.
        /// </returns>
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
                    return StatusCode(200, new Response<object>() { IsSuccess = true, Message = "Your password has been changed."});
                }
                else
                {
                    return StatusCode(400, new Response<object>() { IsSuccess = false, Message = "Unable to change password" });
                }
            }
            else //se for nulo
            {
                return StatusCode(404, new Response<object>() { IsSuccess = false, Message = "Unable to change password" });
            }
        }


        /// <summary>
        /// Retrieves a user's DTO (Data Transfer Object) based on their email.
        /// This endpoint requires a valid JWT token.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with the user's DTO if the user is found,
        /// otherwise, a 404 Not Found response.
        /// </returns>
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



        /// <summary>
        /// Retrieves a user's DTO (Data Transfer Object) based on their email.
        /// This endpoint requires a valid JWT token.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with the user's DTO if the user is found,
        /// otherwise, a 404 Not Found response.
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUserByEmail2")]
        public async Task<IActionResult> GetUserByEmail2([FromQuery] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = _converterHelper.ToUserDto(user);

            return Ok(userDto);
        }


        /// <summary>
        /// Allows a logged-in user to update their profile information.
        /// </summary>
        /// <param name="userDto">The DTO containing the updated user information.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with the updated user DTO if the update is successful,
        /// otherwise, a 404 Not Found or 500 Internal Server Error response.
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpPost("EditProfile")]

        public async Task<IActionResult> EditProfile([FromBody] UserDto userDto)
        {
            var user = await _converterHelper.ToEditedProfile(userDto);

            if (user == null)
            {
                return NotFound(null);
            }

            var response = await _userHelper.UpdateUserAsync(user);


            if (response.Succeeded)
            {
                var editedUser = await _userHelper.GetUserByEmailAsync(userDto.Email);

                var editedUserDto = _converterHelper.ToUserDto(editedUser);

                if (await _userHelper.IsUserInRoleAsync(user, "CondoMember"))
                {
                    var condomember = await _converterHelper.FromUserToCondoMember(user);

                    if (condomember == null)
                    {
                        return NotFound();
                    }

                    await _condoMemberRepository.UpdateAsync(condomember, _dataContextCondos);

                    return Ok(editedUserDto);
                }

                return Ok(editedUserDto);
            }

            return StatusCode(500, new { error = "An internal server error occurred." });
        }


        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpPost("EditUserDetails")]
        //public async Task<IActionResult> EditUserDetails([FromBody] EditUserDetailsDto editUserDetailsDto)
        //{
        //    var user = await _userHelper.GetUserByEmailAsync(editUserDetailsDto.Email);

        //    if (user == null)
        //    {
        //        return NotFound(null);  
        //    }

        //    await _userHelper.UpdateUserAsync(user);

        //    var editedUser = await _userHelper.GetUserByEmailAsync(user.Email);

        //    if (user == null)
        //    {
        //        return NotFound(null);
        //    }

        //    return Ok(editedUser);
        //}


        /// <summary>
         /// Retrieves a list of users whose full name matches the provided search query.
         /// </summary>
         /// <param name="userFullName">The full name to search for, passed as a query parameter.</param>
         /// <returns>A list of UserDto objects that match the search criteria. Returns an empty list if the query is null or empty.</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUsersByFullName")]
        public async Task<List<UserDto>> GetUsersByFullName([FromQuery] string userFullName) //pegar parametro da query, depois do [?]
        {
            if (string.IsNullOrEmpty(userFullName))
            {
                return new List<UserDto>(); // retorna lista vazia
            }

            string cleanedFullName = userFullName.Trim().ToLower();

            var users = await _userHelper.GetUsersByFullName(cleanedFullName);

            var usersDto = users.Select(u => _converterHelper.ToUserDto(u)).ToList();

            return usersDto;
        }


        /// <summary>
         /// Edits the details of an existing user and, if the user is a "CondoMember," updates their associated condo details.
         /// </summary>
         /// <param name="editUserDetailsDto">The data transfer object containing the user details to be updated.</param>
         /// <returns>
         /// An IActionResult indicating the result of the operation.
         /// Returns NotFound if the user is not found, otherwise returns Ok on successful update.
         /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("EditUserDetails")]
        public async Task<IActionResult> EditUserDetails([FromBody] EditUserDetailsDto editUserDetailsDto)
        {
            var editedUser = await _converterHelper.ToEditedUser(editUserDetailsDto);

            if (editedUser == null)
            {
                return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to update, user not found in the system"});
            }

            await _userHelper.UpdateUserAsync(editedUser);

            if (await _userHelper.IsUserInRoleAsync(editedUser, "CondoMember"))
            {
                var condomember = await _converterHelper.FromUserToCondoMember(editedUser);

                if (condomember == null)
                {
                    return NotFound(new Response<object>  { IsSuccess = false, Message = "Unable to update, user not found in the system" });
                }

                await _condoMemberRepository.UpdateAsync(condomember, _dataContextCondos);

                return Ok(new Response<object> { IsSuccess = true });
            }

            return Ok(new Response<object> { IsSuccess = true});
        }


        /// <summary>
         /// Retrieves a list of all users assigned to a specific role.
         /// </summary>
         /// <param name="role">The name of the role to search for, passed in the request body.</param>
         /// <returns>A collection of UserDto objects for all users found with the specified role.</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetAllUsersByRole")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersByRole([FromBody] string role)
        {
            var users = await _userHelper.GetUsersByRoleAsync($"{role}");

            var usersDto = users.Select(u => _converterHelper.ToUserDto(u)).ToList();

            return usersDto;

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUsersWithCompany")]
        public async Task<IActionResult> GetUsersWithCompanyAsync([FromQuery] string role )
        {

            var users = await _userHelper.GetUsersWithCompanyAsync();

            var filteredUsers = new List<User>();

            foreach (var user in users)
            {
                if (await _userHelper.IsUserInRoleAsync(user,role))
                {
                    filteredUsers.Add(user);
                }
            }

            users = filteredUsers;

            if (users == null || !users.Any())
                return NotFound("No users found");

            var usersDto = users.Select(u => _converterHelper.ToUserDto(u)).ToList();
            return Ok(usersDto);
        }


        [HttpGet("GetManagers")]
        public async Task<IActionResult> GetManagers()
        {

            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity?.Name);

            var allManagers = await _userHelper.GetUsersByRoleAsync("CondoManager");

            var managers = new List<User>();

            managers = allManagers
            .Where(m => m.CompanyId == user.CompanyId)
            .ToList();


            var managersDto = managers
                .Select(m => _converterHelper.ToUserDto((User)m))
                .ToList();

            return Ok(managersDto);
        }

    }
}


