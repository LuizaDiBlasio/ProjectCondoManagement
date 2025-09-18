using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Identity;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Helpers
{
    public interface IUserHelper
    {

        Task<User> CreateUser(RegisterUserDto registerDtoModel); //cria user com base no RegisterUserDto

        Task<User> GetUserByEmailAsync(string email); //passa o email para buscar user

        Task<User> GetUserByEmailWithCompaniesAsync(string email); //passa o email para buscar user com a empresa associada

        Task<List<User>> GetUsersByEmailsAsync(IEnumerable<string> emails); //recebe uma lista de emails e devolve uma lista de users 

        Task<IdentityResult> AddUserAsync(User user, string password); //adiciona user na BD

        Task<SignInResult> LoginAsync(LoginDto model); //método que devolve tarefa SignInResult (ou tá signed in ou não)

        Task<string> GenerateTwoFactorTokenAsync(User user, string tokenProvider); // criação de token 2FA

        Task<IdentityResult> EnableTwoFactorAuthenticationAsync(User user, bool enable); // habilita 2FA

        Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenProvider, string token);

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user); //update user na BD

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword); //muda pass do user 

        Task CheckRoleAsync(string roleName); //verifica se existem roles

        Task AddUserToRoleAsync(User user, string roleName); // designa o role ao user

        Task<bool> IsUserInRoleAsync(User user, string roleName); // verifica se user está designado ao role

        Task CreateRolesAsync(); //cria roles

        Task<Response<object>> DeactivateUserAsync(User user);

        Task<SignInResult> ValidatePasswordAsync(User user, string password); //não faz login, só valida a password para acesso à API

        Task<string> GenerateEmailConfirmationTokenAsync(User user); //Gera o email de confirmação e insere o Token 

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token); //Valida o email, verifica se o token é valido

        Task<User> GetUserByIdAsync(string id); //recebe um id e devolve o user correspondente

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password); //Faz o reset da password

        Task<IList<string>> GetRolesAsync(User user); //busca o role do user

        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);

        Task<List<User>> GetUsersByFullName(string cleanedFullName);
        Task<List<User>> GetUsersWithCompanyAsync();

        Task<bool> ExistsAsync(string email);

        Task<List<User>> GetUsersWithCompanyByRoleAsync(string role);

        Task<User> GetUserByIdWithCompaniesAsync(string id);
        Task<User> GetUserByFinancialAccountIdAsync(int financialAccountId);
    }
}
