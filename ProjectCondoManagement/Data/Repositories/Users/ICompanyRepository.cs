using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.UsersDb;

namespace ProjectCondoManagementAPI.Data.Repositories.Users
{
    public interface ICompanyRepository : IGenericRepository<Company, DataContextUsers>
    {
    }
}
