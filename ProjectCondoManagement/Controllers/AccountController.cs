using ClassLibrary;
using ClassLibrary.DtoModels;
using ClassLibrary.DtoModelsMobile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Data.Repositories.Users;
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
        private readonly DataContextUsers _dataContextUsers;
        private readonly ICompanyRepository _companyRepository;
        private readonly UserManager<User> _userManager;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMeetingRepository _meetingRepository;

        public AccountController(IUserHelper userHelper, HttpClient httpClient, IConfiguration configuration, IConverterHelper converterHelper,
                               IMailHelper mailHelper, DataContextCondos dataContextCondos, DataContextFinances dataContextFinances, IJwtTokenService jwtTokenService,
                               ICondoMemberRepository condoMemberRepository, ISmsHelper smsHelper, IFinancialAccountRepository financialAccountRepository, IWebHostEnvironment env,
                               DataContextUsers dataContextUsers, ICompanyRepository companyRepository, UserManager<User> userManager, ICondominiumRepository condominiumRepository,
                               IPaymentRepository paymentRepository, IMessageRepository messageRepository, IMeetingRepository meetingRepository)
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
            _dataContextUsers = dataContextUsers;
            _companyRepository = companyRepository;
            _userManager = userManager;
            _condominiumRepository = condominiumRepository;
            _paymentRepository = paymentRepository;
            _messageRepository = messageRepository;
            _meetingRepository = meetingRepository;
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


            if (result.RequiresTwoFactor) //se login for bem sucedido e precisar de 2fa 
            {

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
                // Para testes em desenvolvimento___________________________________________
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
            }

            return StatusCode(500, new { Message = "Wrong credentials, please try again" });
       
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
                return StatusCode(404, new { Message = "User not found", IsSuccess = false });
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


        [HttpPost("LoginMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginMobile([FromBody] LoginMobileDto model)
        {
            // Validação do modelo de entrada
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid credentials." });
            }

            // Autenticação do usuário
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { isSuccess = false, message = "Invalid email or password." });
            }

            // 3. Obter a role do usuário
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                return BadRequest(new { isSuccess = false, message = "User has no role." });
            }
            var role = roles.First();

            // Gerar o JWT usando seu serviço
            var token = _jwtTokenService.GenerateToken(user, role);

            // Retornar a resposta de sucesso com o token
            return Ok(new LoginResponseDto ()
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = token,
                UserRole = role
            });
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
            var user = await _userHelper.GetUserByEmailAsync(registerDtoModel.Email); //buscar user  

            if (user != null)
            {
                return StatusCode(409, new Response<object> { Message = "User already exists, try registering wih new credentials", IsSuccess = false });
            }

            user = await _userHelper.CreateUser(registerDtoModel);
            if (user == null)
            {
                return StatusCode(500, new { Message = "Internal server error: User not registered" });
            }

            await _condoMemberRepository.AssociateFinancialAccountAsync(user.Email, user.FinancialAccountId);


            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //gerar o token

            // gera um link de confirmção para o email
            string tokenLink = $"{_configuration["WebAppSettings:BaseUrl"]}/Account/ResetPassword?userId={user.Id}&tokenEmail={Uri.EscapeDataString(myToken)}"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response<object> response = _mailHelper.SendEmail(registerDtoModel.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
           $"To allow the user,<br><br><a href = \"{tokenLink}\">Click here to confirm your email and reset password</a>"); //Contruir email e enviá-lo com o link



            if (response.IsSuccess) //se conseguiu enviar o email
            {


                return StatusCode(200, new Response<object> { Message = "User registered, a confirmation email has been sent", IsSuccess = true });
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
                return Ok(new Response<object> { Message = "User already exists, try registering wih new credentials", IsSuccess = false });
            }
            else //criar user caso não exista
            {
                var newUser = await _userHelper.CreateUser(registerDtoModel);

                if (newUser == null)//se retorna null, CreateUser foi mal sucedido 
                {
                    return StatusCode(500, new { Message = "Internal server error: User not registered" });
                }


                //atualizar company admin Ids das companies
                if (await _userHelper.IsUserInRoleAsync(newUser, "CompanyAdmin"))
                {
                    foreach (var company in newUser.Companies)
                    {
                        company.CompanyAdminId = newUser.Id;

                        await _companyRepository.UpdateAsync(company, _dataContextUsers);
                    }
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
                        BirthDate = newUser.BirthDate,
                        FinancialAccountId = newUser.FinancialAccountId.Value
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
                    return StatusCode(200, new Response<object>() { IsSuccess = true, Message = "Your password has been changed." });
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
            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            if (user == null)
            {
                return NotFound(null);
            }

            var userDto = _converterHelper.ToUserDto(user, true);

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
            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = _converterHelper.ToUserDto(user, true);

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

                var editedUserDto = _converterHelper.ToUserDto(editedUser, true);

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

            var usersDto = users.Select(u => _converterHelper.ToUserDto(u, true)).ToList();

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
            var editedUser = await _converterHelper.ToEditedUser(editUserDetailsDto, true);

            if (editedUser == null)
            {
                return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to update, user not found in the system" });
            }

            //desfazer relações com companies
            if (editedUser.IsActive == false)
            {
                editedUser.Companies.Clear();
            }

            await _userHelper.UpdateUserAsync(editedUser);

            if (await _userHelper.IsUserInRoleAsync(editedUser, "CondoMember"))
            {
                var condomember = await _converterHelper.FromUserToCondoMember(editedUser);

                if (condomember == null)
                {
                    return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to update, user not found in the system" });
                }

                await _condoMemberRepository.UpdateAsync(condomember, _dataContextCondos);

                return Ok(new Response<object> { IsSuccess = true });
            }

            return Ok(new Response<object> { IsSuccess = true });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AssignCompanyToMember")]
        public async Task<IActionResult> AssingCompanyToMember([FromBody] UserDto userDto)
        {
            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(userDto.Email);

            if (user == null)
            {
                NotFound();
            }

            var userCompaniesIds = userDto.CompaniesDto.Select(c => c.Id).ToList();

            var userCompanies = new List<Company>();

            foreach (var companyId in userCompaniesIds)
            {
                var company = _dataContextUsers.Companies.FirstOrDefault(c => c.Id == companyId);
                userCompanies.Add(company);
            }

            user.Companies.Clear();

            user.Companies = userCompanies;

            await _userHelper.UpdateUserAsync(user);

            return Ok(new Response<object> { IsSuccess = true });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AssignCompanyToAdminOrManager")]
        public async Task<IActionResult> AssingCompanyToAdminOrManager([FromBody] AssignmentDto assignmentDto)
        {
            try
            {
                if (assignmentDto.CompanyDto == null)
                {
                    return BadRequest("Request body is null.");
                }

                //buscar company antes de fazer assign 

                var company = await _companyRepository.GetCompanyWithCondosAndUsers(assignmentDto.CompanyDto.Id);
                if (company == null)
                {
                    return Ok(new Response<object> { IsSuccess = false, Message = "Unable to assing user, company not found" });
                }

                // ver se user é condoManager ou company admin  

                var user = await _userHelper.GetUserByEmailWithCompaniesAsync(assignmentDto.UserDto.Email);

                if (await _userHelper.IsUserInRoleAsync(user, "CompanyAdmin"))
                {
                    if (company.CompanyAdminId != null)
                    {
                        var oldCompanyAdminUser = await _userHelper.GetUserByIdWithCompaniesAsync(company.CompanyAdminId);

                        //atualizar dados do novo admin
                        if (oldCompanyAdminUser != null && oldCompanyAdminUser.Id != user.Id)
                        {
                            user.Companies.Clear();
                            user.Companies.Add(company);


                            company.CompanyAdminId = user.Id;

                            await _userHelper.UpdateUserAsync(user);
                            await _companyRepository.UpdateAsync(company, _dataContextUsers);
                        }
                        else
                        {
                            return Ok(new Response<object> { IsSuccess = false, Message = "User already assigned to this company" });
                        }
                    }
                    else
                    {
                        company.CompanyAdminId = assignmentDto.CompanyDto.CompanyAdminId;

                        company.Users.Add(user);

                        await _companyRepository.UpdateAsync(company, _dataContextUsers);
                    }
                }
                else //será condo manager, relação 1:N (1 company - N users), atualizar pelo lado de users
                {
                    user.Companies.Clear();
                    user.Companies.Add(company);

                    await _userHelper.UpdateUserAsync(user);
                }

                return Ok(new Response<object> { IsSuccess = true, Message = "User assigned successfully" });
            }
            catch
            {
                return Ok(new Response<object> { IsSuccess = false, Message = "Unable to assing user due to server error" });
            }

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

            var usersDto = users.Select(u => _converterHelper.ToUserDto(u, true)).ToList();

            return usersDto;

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUsersWithCompany")]
        public async Task<IActionResult> GetUsersWithCompanyAsync([FromQuery] string role)
        {

            var users = await _userHelper.GetUsersWithCompanyAsync();

            var filteredUsers = new List<User>();

            foreach (var user in users)
            {
                if (await _userHelper.IsUserInRoleAsync(user, role))
                {
                    filteredUsers.Add(user);
                }
            }


            var usersDto = filteredUsers.Select(u => _converterHelper.ToUserDto(u, true)).ToList();


            return Ok(usersDto);
        }


        [HttpGet("GetManagers")]
        public async Task<IActionResult> GetManagers()
        {
            var managers = await _userHelper.GetUsersWithCompanyByRoleAsync("CondoManager");

            if (!this.User.IsInRole("SysAdmin"))
            {
                var currentUserEmail = User.Identity?.Name;
                var currentUser = await _userHelper.GetUserByEmailWithCompaniesAsync(currentUserEmail);
                if (currentUser == null)
                {
                    return Unauthorized(new { Message = "Current user not found." });
                }

                var userCompaniesIds = currentUser.Companies.Select(c => c.Id).ToList();

                managers = managers.Where(m => m.Companies.Any(c => userCompaniesIds.Contains(c.Id))).ToList();

            }

            var managersDto = managers
                .Select(m => _converterHelper.ToUserDto((User)m, true))
                .ToList();

            return Ok(managersDto);

        }

        [HttpGet("GetUserRole")]
        public async Task<ActionResult<string>> GetUserRole([FromQuery] string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);

            var userRoles = await _userHelper.GetRolesAsync(user);

            var userRole = new UserRoleDto()
            {
                Role = userRoles.First()
            };

            return Ok(userRole);
        }





        [HttpGet("CondoManagerDashboard/{email}")]
        public async Task<ActionResult<CondoManagerDashboardDto>> GetCondoManagerDashboard(string email)
        {
            var model = new CondoManagerDashboardDto();


            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            var userDto = _converterHelper.ToUserDto(user, true);

            model.CondoManager = userDto;

            var condos = await _condominiumRepository.GetAll(_dataContextCondos).Where(c => c.ManagerUserId == user.Id)
                                                      .Include(c => c.Occurrences).Include(c => c.Units).ToListAsync();

            var condosDto = condos.Select(c => _converterHelper.ToCondominiumDto(c, true)).ToList();

            var ocurrencesDtos = condosDto.SelectMany(c => c.Occurrences);

            model.Condominiums = condosDto;

            model.Occurrences = ocurrencesDtos.ToList();


            var condoIds = condos.Select(c => c.Id);

            var payments = await _paymentRepository.GetAll(_dataContextFinances)
                .Where(p => condoIds.Contains(p.CondominiumId) && !p.IsPaid)
                .ToListAsync();

            var paymentsDto = payments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList();

            model.Payments = paymentsDto;

            var messages = await _messageRepository.GetAll(_dataContextUsers)
                .Where(m => m.ReceiverEmail == email)
                .ToListAsync();

            var messagesDto = messages.Select(m => _converterHelper.ToMessageDto(m,null)).ToList();

            model.Messages = messagesDto;

            var meetings = await _meetingRepository.GetAll(_dataContextCondos)
                .Where(m => condoIds.Contains(m.CondominiumId) && m.DateAndTime >= DateTime.Today)
                .ToListAsync();

            var meetingsDto = meetings.Select(m => _converterHelper.ToMeetingDto(m, false)).ToList();
            model.Meetings = meetingsDto;


            model.Meetings = model.Meetings.OrderBy(m => m.DateAndTime).ToList();

            return Ok(model);
        }



        [HttpGet("CompanyAdminDashboard/{email}")]
        public async Task<ActionResult<CompanyAdminDashboardDto>> GetCompanyAdminDashboard(string email)
        {
            var model = new CompanyAdminDashboardDto();


            var user = await _userHelper.GetUserByEmailWithCompaniesAsync(email);

            var userDto = _converterHelper.ToUserDto(user, true);

            model.CompanyAdmin = userDto;

            var financialAccountId = user.Companies
                .Select(c => c.FinancialAccountId)
                .FirstOrDefault();

            var companyId = user.Companies
                .Select(c => c.Id)
                .FirstOrDefault();

            var financialAccount = financialAccountId != 0
                ? await _financialAccountRepository.GetByIdAsync(financialAccountId, _dataContextFinances)
                : null;


            var financialAccountDto = financialAccount != null
                ? _converterHelper.ToFinancialAccountDto(financialAccount, false)
                : null;


            model.FinancialAccount = financialAccountDto ?? new FinancialAccountDto();


            var payments = await _paymentRepository.GetAll(_dataContextFinances)
                   .Where(p => p.BeneficiaryAccountId == financialAccountDto.Id && p.PayerFinancialAccountId == financialAccountDto.Id && !p.IsPaid)
                   .ToListAsync();

            var paymentsDto = payments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList();

            model.Payments = paymentsDto;



            var condos = await _condominiumRepository.GetAll(_dataContextCondos).Where(c => c.CompanyId == companyId)
                                                      .Include(c => c.Occurrences).Include(c => c.Units).ToListAsync();

            var condosDto = condos.Select(c => _converterHelper.ToCondominiumDto(c, true)).ToList();

            model.Condominiums = condosDto;

            var condoManagers = _userHelper.GetUsersByRoleAsync("CondoManager");

            var filteredManagers = condoManagers.Result.Where(m => m.Companies.Any(c => c.Id == companyId)).ToList();

            model.CondoManagers = filteredManagers.Select(m => _converterHelper.ToUserDto(m, true)).ToList();


            var messages = await _messageRepository.GetAll(_dataContextUsers)
                .Where(m => m.ReceiverEmail == email)
                .ToListAsync();

            var messagesDto = messages.Select(m => _converterHelper.ToMessageDto(m, null)).ToList();

            model.Messages = messagesDto;


            return Ok(model);
        }



    }
}


