using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public interface ICompanyRepository : IGenericRepository<Company, DataContextUsers>
    {
        Task<Company> GetCompanyWithcCondosAndAdmin(int id, DataContextUsers contextUsers);

        Task<List<SelectListItem>> GetCondosSelectListAsync(DataContextCondos contextCondos);

        Task<List<SelectListItem>> GetCompanyAdminsSelectListAsync();

        Task<bool> ExistingCompany(Company company);

        Task<SelectedAdminAndCondosDto> SelectedAdminAndCondos(CompanyDto companyDto);
    }
}
