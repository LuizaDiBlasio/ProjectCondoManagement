using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, string role);
    }
}
