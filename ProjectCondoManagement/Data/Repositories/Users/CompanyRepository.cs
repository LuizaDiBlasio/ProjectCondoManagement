using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public class CompanyRepository : GenericRepository<Company, DataContextUsers>, ICompanyRepository
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContextUsers _dataContextUsers;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly DataContextCondos _contextCondos;
        private readonly IConverterHelper _converterHelper; 

        public CompanyRepository(IUserHelper userHelper, DataContextUsers contextUsers, ICondominiumRepository condominiumRepository, DataContextCondos contextCondos, IConverterHelper converterHelper)
        {
            _userHelper = userHelper;
            _dataContextUsers = contextUsers;   
            _condominiumRepository = condominiumRepository;
            _contextCondos = contextCondos; 
            _converterHelper = converterHelper;
        }

        

        public async Task<List<SelectListItem>> GetCompanyAdminsSelectListToEdit(int id)
        {
            var companyAdmins = await _userHelper.GetUsersWithCompanyByRoleAsync("CompanyAdmin");

            if(companyAdmins == null)
            {
                return null;
            }

            var availableAdmins = companyAdmins.Where(u => !u.Companies.Any());

            var adminsToSelect = availableAdmins
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName
                })
                .OrderBy(s => s.Text)
                .ToList();


            //buscar o admin já selecionado para mostrar na seleção:

            var company = await GetByIdAsync(id, _dataContextUsers);
            if(company != null)
            {
                var adminSelecionado = companyAdmins.FirstOrDefault(ca => ca.Id == company.CompanyAdminId);
                if (adminSelecionado != null)
                {
                    var adminSelecionadoItem = new SelectListItem()
                    {
                        Value = adminSelecionado.Id.ToString(),
                        Text = adminSelecionado.FullName
                    };

                    adminsToSelect.Add(adminSelecionadoItem);
                }
            }
           
            // Apenas retorna a lista
            return adminsToSelect;
        }


        public async Task<List<SelectListItem>> GetCompanyAdminsSelectListAsync()
        {
            var companyAdmins = await _userHelper.GetUsersWithCompanyByRoleAsync("CompanyAdmin");

            if (companyAdmins == null)
            {
                return null;
            }

            var availableAdmins = companyAdmins.Where(u => !u.Companies.Any());

            var adminsToSelect = availableAdmins
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName
                })
                .OrderBy(s => s.Text)
                .ToList();

            // Apenas retorna a lista
            return adminsToSelect;
        }


        public async Task<bool> ExistingCompany(Company company)
        {
            return await _dataContextUsers.Companies.AnyAsync(c => c.TaxIdDocument == company.TaxIdDocument);
        }


        public async Task<UserDto> SelectedAdmin(CompanyDto companyDto)
        {
            var admins = await _userHelper.GetUsersByRoleAsync("CompanyAdmin");

            var adminsDto = admins?.Select(a => _converterHelper.ToUserDto(a, true)).ToList() ?? new List<UserDto>();

            var selectedAdmin = adminsDto.FirstOrDefault(a => a.Id == companyDto.CompanyAdminId);

            return selectedAdmin;
                
        }

        public async Task<Company?> GetCompanyWithCondosAndUsers(int id)
        {
            
            var company = await _dataContextUsers.Companies.Include(c => c.Users).FirstOrDefaultAsync(c => c.Id == id);
            
            if (company == null)
            {
                return null;
            }
            else
            {
                var companyCondos = await _condominiumRepository.GetAll(_contextCondos)
                                    .Where(c => c.CompanyId == id)
                                    .ToListAsync();

                company.Condominiums = companyCondos;
              
                return company; 
            }
        }

        public async Task<Company> GetCompanyByFinancialAccountIdAsync(int id)
        {

            return await _dataContextUsers.Companies.FirstOrDefaultAsync(c => c.FinancialAccountId == id);

        }
    }
}
