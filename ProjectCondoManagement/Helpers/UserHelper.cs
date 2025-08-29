using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly DataContextUsers _dataContextUsers;

        private readonly IFinancialAccountRepository _financialAccountRepository;

        private readonly DataContextFinances _dataContextFinances;



        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager,
            IFinancialAccountRepository financialAccountRepository, DataContextFinances dataContextFinances, DataContextUsers dataContextUsers)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dataContextUsers = dataContextUsers;
            _financialAccountRepository = financialAccountRepository;
            _dataContextFinances = dataContextFinances;
        }


        public async Task<User> CreateUser(RegisterUserDto registerDtoModel)
        {
            if (registerDtoModel == null) // se o modelo for nulo, retorna nulo
            {
                return null;
            }

            var user = await GetUserByEmailAsync(registerDtoModel.Email); //buscar user  
            if (user != null)
            {
                return null; //já existe o user --> resposta negativa (null)
            }

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
                FinancialAccountId = null,
            };

            var result = await AddUserAsync(user, "123456"); //add user depois de criado

            if (result != IdentityResult.Success) // caso não consiga criar user
            {
                return null;
            }

            //Adicionar roles ao user
            switch (registerDtoModel.SelectedRole)
            {
                case "CondoMember":
                    await AddUserToRoleAsync(user, "CondoMember");
                    break;
                case "CondoManager":
                    await AddUserToRoleAsync(user, "CondoManager");
                    break;
                case "CompanyAdmin":
                    await AddUserToRoleAsync(user, "CompanyAdmin");
                    break;
            }

            //se for condoMember adicionanr uma financial account

            if(await IsUserInRoleAsync(user, "CondoMember"))
            {
                var financialAccount = new FinancialAccount()
                {
                    Balance = 0,
                    OwnerName = user.FullName,
                };

                await _financialAccountRepository.CreateAsync(financialAccount, _dataContextFinances); //add FinAcc na Bd

                user.FinancialAccountId = financialAccount.Id; 
                user.FinancialAccount = financialAccount;
            }

            //TODO Tirar o if e essa atribuição de bool quando publicar, manter só o método de ativação
            user.Uses2FA = true;

            if (user.Uses2FA == true)
            {
                var activation2FA = await EnableTwoFactorAuthenticationAsync(user, true);
            }


            return user;
        }

        /// <summary>
        /// Creates a new user with the specified password.
        /// </summary>
        /// <param name="user">The "User" entity to create.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains an "IdentityResult" indicating the success or failure of the operation.
        /// </returns>
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }


        /// <summary>
        /// Asynchronously adds a user to a specified role.
        /// </summary>
        /// <param name="user">The "User" entity to add to the role.</param>
        /// <param name="roleName">The name of the role to add the user to.</param>
        /// <returns>A "Task" that represents the asynchronous operation.</returns>
        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }


        /// <summary>
        /// Asynchronously changes the password for a user.
        /// </summary>
        /// <param name="user">The "User" entity whose password will be changed.</param>
        /// <param name="oldPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password for the user.</param>
        /// <returns>
        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }


        /// <summary>
        /// Asynchronously checks if a role exists, if it doesn't it creates the role.
        /// </summary>
        /// <param name="roleName">The name of the role to check.</param>
        /// <returns>A "Task" that represents the asynchronous operation.</returns>
        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName); // se existe, buscar o role

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });

            }

        }


        /// <summary>
        /// Creates predefined roles ("Employee", "Student", "Admin") if they do not already exist in the system.
        /// </summary>
        /// <returns>A "Task" that represents the asynchronous operation.</returns>
        public async Task CreateRolesAsync()
        {
            string[] roleNames = { "CondoManager", "CondoMember", "CompanyAdmin", "SysAdmin" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }



        /// <summary>
        /// Asynchronously retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>
        /// A "Task{TResult} that represents the asynchronous operation.
        /// The task result contains the "User" entity if found, otherwise "null".
        /// </returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> GetUserByEmailWithCompanyAsync(string email)
        {
            return await _dataContextUsers.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetUsersByEmailsAsync(IEnumerable<string> emails)
        {
            return await _dataContextUsers.Users
                .Where(u => emails.Contains(u.Email))
                .ToListAsync();
        }



        /// <summary>
        /// Asynchronously checks if a user is a member of a specified role.
        /// </summary>
        /// <param name="user">The "User" entity to check.</param>
        /// <param name="roleName">The name of the role to check against.</param>
        /// <returns>
        /// A "Task{TResult} that represents the asynchronous operation.
        /// The task result contains "true" if the user is in the specified role, otherwise "false".
        /// </returns>
        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName); //devolve uma booleana dizendo se user está no role ou não
        }


        /// <summary>
        /// Asynchronously attempts to sign in a user with the provided credentials.
        /// </summary>
        /// <param name="model">The "LoginViewModel" containing the username, password, and remember me preference.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains a "SignInResult" indicating the outcome of the sign-in attempt.
        /// </returns>
        public async Task<SignInResult> LoginAsync(LoginDto model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
        }


        /// <summary>
        /// Asynchronously signs out the currently authenticated user.
        /// </summary>
        /// <returns>A "Task" that represents the asynchronous operation.</returns>
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        /// <summary>
        /// Asynchronously updates the user's information in the data store.
        /// </summary>
        /// <param name="user">The "User" entity with updated properties.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains an "IdentityResult" indicating the success or failure of the update.
        /// </returns>
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }


        /// <summary>
        /// Asynchronously generates an email confirmation token for the specified user.
        /// </summary>
        /// <param name="user">The "User" entity for whom to generate the token.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains the generated email confirmation token.
        /// </returns>
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }


        /// <summary>
        /// Asynchronously generates a password reset token for the specified user.
        /// </summary>
        /// <param name="user">The "User" entity for whom to generate the token.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains the generated password reset token.
        /// </returns>
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }


        /// <summary>
        /// Asynchronously confirms the user's email address using a token.
        /// </summary>
        /// <param name="user">The "User" entity whose email is to be confirmed.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains an "IdentityResult" indicating the success or failure of the confirmation.
        /// </returns>
        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);

        }


        /// <summary>
        /// Asynchronously retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains the "User" entity if found, otherwise "null".
        /// </returns>
        public Task<User> GetUserByIdAsync(string id)
        {
            return _userManager.FindByIdAsync(id);
        }


        /// <summary>
        /// Asynchronously resets a user's password using a password reset token.
        /// </summary>
        /// <param name="user">The "User" entity whose password is to be reset.</param>
        /// <param name="token">The password reset token.</param>
        /// <param name="password">The new password for the user.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains an "IdentityResult" indicating the success or failure of the password reset.
        /// </returns>
        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }


        /// <summary>
        /// Asynchronously validates a user's password without signing them in.
        /// </summary>
        /// <param name="user">The "User" entity whose password is to be validated.</param>
        /// <param name="password">The password to validate.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains a "SignInResult" indicating whether the password is valid.
        /// </returns>
        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }


        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<Response<object>> DeactivateUserAsync(User user)
        {
            user.IsActive = false;

            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new Response<object>
                    {
                        IsSuccess = true,
                    };
                }

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = "User deactivation failed.",
                };
            }
            catch (Exception ex)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Error deactivating User.({ex.Message})"
                };
            }

        }


        public async Task<List<User>> GetUsersByFullName(string cleanedFullName)
        {
            return await _dataContextUsers.Users
               .Where(s => s.FullName.ToLower() == cleanedFullName)
               .ToListAsync();
        }


        public async Task<string> GenerateTwoFactorTokenAsync(User user, string tokenProvider)
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
        }

        public async Task<IdentityResult> EnableTwoFactorAuthenticationAsync(User user, bool enable)
        {
            return await _userManager.SetTwoFactorEnabledAsync(user, true);
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenProvider, string token)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, tokenProvider, token);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                return await _userManager.GetUsersInRoleAsync(role);
            }

            return new List<User>();
        }




    }
}
