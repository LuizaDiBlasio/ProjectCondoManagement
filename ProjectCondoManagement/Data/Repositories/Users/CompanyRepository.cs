using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.UsersDb;

namespace ProjectCondoManagementAPI.Data.Repositories.Users
{
    public class CompanyRepository : GenericRepository<Company, DataContextUsers>, ICompanyRepository
    {
    }
}
