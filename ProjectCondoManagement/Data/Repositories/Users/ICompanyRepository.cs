using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public interface ICompanyRepository : IGenericRepository<Company, DataContextUsers>
    {
        Task<List<SelectListItem>> GetCompanyAdminsSelectListAsync();

        Task<bool> ExistingCompany(Company company);

        Task<UserDto> SelectedAdmin(CompanyDto companyDto);

        Task<List<SelectListItem>> GetCompanyAdminsSelectListToEdit(int id);

        Task<Company?> GetCompanyWithCondosAndUsers(int id);
    }
}
