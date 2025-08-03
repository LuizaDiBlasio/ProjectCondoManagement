using ClassLibrary;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public interface ICompanyRepository : IGenericRepository<Company, DataContextUsers>
    {
    }
}
