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

        

        public async Task<List<SelectListItem>> GetCondosSelectListAsync(DataContextCondos contextCondos)
        {
            var allCondos = await contextCondos.Condominiums.ToListAsync();

            var availableCondos = allCondos
                                            .Select(s => new SelectListItem
                                            {
                                                Value = s.Id.ToString(),
                                                Text = s.CondoName
                                            })
                                            .OrderBy(s => s.Text).ToList();

            // Apenas retorna a lista
            return availableCondos;
        }


        public async Task<List<SelectListItem>> GetCondosSelectListAsyncToCreate(DataContextCondos contextCondos)
        {
            var allCondos = await contextCondos.Condominiums.ToListAsync();

            var availableCondos = allCondos.Where(c => c.CompanyId == 0) // ainda não selecionados
                                            .Select(s => new SelectListItem
                                            {
                                                Value = s.Id.ToString(),
                                                Text = s.CondoName
                                            })
                                            .OrderBy(s => s.Text).ToList();

            // Apenas retorna a lista
            return availableCondos;
        }


        public async Task<List<SelectListItem>> GetCompanyAdminsSelectListToEdit(int id)
        {
            var companyAdmins = await _userHelper.GetUsersByRoleAsync("CompanyAdmin");

            if(companyAdmins == null)
            {
                return null;
            }

            var availableAdmins = companyAdmins.Where(u => u.CompanyId == null);

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
            var companyAdmins = await _userHelper.GetUsersByRoleAsync("CompanyAdmin");

            if (companyAdmins == null)
            {
                return null;
            }

            var availableAdmins = companyAdmins.Where(u => u.CompanyId == null);

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


        public async Task<SelectedAdminAndCondosDto> SelectedAdminAndCondos(CompanyDto companyDto)
        {
            var admins = await _userHelper.GetUsersByRoleAsync("CompanyAdmin");

            var adminsDto = admins?.Select(a => _converterHelper.ToUserDto(a)).ToList() ?? new List<UserDto>();    

            var selectedAdmin = adminsDto.FirstOrDefault(a => a.Id == companyDto.CompanyAdminId);


            var selectedCondos = new List<CondominiumDto> ();

            var condos = _condominiumRepository.GetAll(_contextCondos);

            var condosDto = condos?.Select(c => _converterHelper.ToCondominiumDto(c, false)).ToList() ?? new List<CondominiumDto>();

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
