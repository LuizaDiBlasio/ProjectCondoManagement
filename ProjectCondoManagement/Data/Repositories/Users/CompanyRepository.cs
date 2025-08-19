using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
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

        public async Task<Company> GetCompanyWithcCondosAndAdmin(int id, DataContextUsers contextUsers)
        {
            //buscar company
            var company= await contextUsers.Companies.
                Where(c => c.Id == id)
                .Include(c => c.CompanyAdmin)
                .FirstOrDefaultAsync();

            //buscar condos da company
           
            var  companyCondos = await _contextCondos.Condominiums.
                Where(c => c.CompanyId == id).ToListAsync();    
            
            if(company != null)
            {
                company.Condominiums = companyCondos;
                return company;
            }

            return null;
        }

        public async Task<List<SelectListItem>> GetCondosSelectListAsync(DataContextCondos contextCondos)
        {
            var condosToSelect = await contextCondos.Condominiums
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.CondoName
                })
                .OrderBy(s => s.Text)
                .ToListAsync();

            // Apenas retorna a lista
            return condosToSelect;
        }

        public async Task<List<SelectListItem>> GetCompanyAdminsSelectListAsync()
        {
            var companyAdmins = await _userHelper.GetUsersByRoleAsync("CompanyAdmin");

            if(companyAdmins == null)
            {
                return null;
            }

            var adminsToSelect = companyAdmins
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


        public async Task<SelectedAdminAndCondosDto> SelectedAdminAndCondos(CompanyDto companyDto)
        {
            var admins = await _userHelper.GetUsersByRoleAsync("CompanyAdmin");

            var adminsDto = admins?.Select(a => _converterHelper.ToUserDto(a)).ToList() ?? new List<UserDto>();    

            var selectedAdmin = adminsDto.FirstOrDefault(a => a.Id == companyDto.CompanyAdminId);


            var selectedCondos = new List<CondominiumDto> ();

            var condos = _condominiumRepository.GetAll(_contextCondos);

            var condosDto = condos?.Select(c => _converterHelper.ToCondominiumDto(c)).ToList() ?? new List<CondominiumDto>();

            foreach(var condo in condosDto)
            {
                if (companyDto.SelectedCondominiumIds != null && companyDto.SelectedCondominiumIds.Any())
                {
                    foreach (var id in companyDto.SelectedCondominiumIds)
                    {
                        if (id == condo.Id)
                        {
                            selectedCondos.Add(condo);
                        }
                    }
                }
               
            }

            var selectedAdminAndCondosDto = new SelectedAdminAndCondosDto()
            {
                SelectedAdmin = selectedAdmin,
                SelectedCondos = selectedCondos   

            };

            return selectedAdminAndCondosDto;


                
        }
        
    }
}
