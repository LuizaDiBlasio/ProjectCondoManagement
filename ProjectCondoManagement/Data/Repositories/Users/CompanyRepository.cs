using ClassLibrary;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public class CompanyRepository : GenericRepository<Company, DataContextUsers>, ICompanyRepository
    {
    }
}
