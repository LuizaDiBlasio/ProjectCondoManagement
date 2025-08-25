using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class CondominiumRepository : GenericRepository<Condominium, DataContextCondos>, ICondominiumRepository
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextCondos _dataContextCondos;

        public CondominiumRepository(IUserHelper userHelper, IConverterHelper converterHelper, DataContextCondos dataContextCondos) 
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _dataContextCondos = dataContextCondos; 
        }

        public async Task<Condominium> GetCondoManagerCondominium(string id)
        {
            return _dataContextCondos.Condominiums.FirstOrDefault(c => c.ManagerUserId == id);

        }


        public async Task<List<Condominium>> GetCompanyCondominiums(List<int> condominiumsIds)
        {
            var companyCondominiums = await _dataContextCondos.Condominiums
                                     .Where(c => condominiumsIds.Contains(c.Id))
                                     .ToListAsync();

            return companyCondominiums;
        }


        public async Task<Response> LinkManager(List<CondominiumDto> condominiums)
        {
            try
            {
                IEnumerable<User> managers = await  _userHelper.GetUsersByRoleAsync("CondoManager");
                if (managers == null || !managers.Any())
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No managers found"
                    };
                }


                foreach (CondominiumDto condominium in condominiums)
                {
                    var manager = managers.FirstOrDefault(m => m.Id == condominium.ManagerUserId);
                    if (manager != null)
                    {
                        var managerDto = _converterHelper.ToUserDto(manager);

                        condominium.ManagerUser = managerDto;
                    }
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = "Images linked successfully"
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = $"Error linking images: {ex.Message}"
                };
            }


        }

        public async Task UpdateCondominiumsCompanyId(Company company)
        {
            var newCondoIds = company.CondominiumIds ?? new List<int>();

            //buscar condos da company
            var currentCompanyCondos = 
                 GetAll(_dataContextCondos)
                .Where(c => c.CompanyId == company.Id)
                .ToList();

            //caso tenha que remover
            var condosToRemove = currentCompanyCondos
                    .Where(c => !newCondoIds.Contains(c.Id))
                    .ToList();

            // fazer update da remoção
            foreach (var condo in condosToRemove)
            {
                condo.CompanyId = null;
                await UpdateAsync(condo, _dataContextCondos);
            }

            //condos adicionados
            var condosToAdd = 
                    GetAll(_dataContextCondos)
                    .Where(c => newCondoIds.Contains(c.Id) && c.CompanyId != company.Id)
                    .ToList();

            // Update adição
            foreach (var condo in condosToAdd)
            {
                condo.CompanyId = company.Id;
                await UpdateAsync(condo, _dataContextCondos);
            }
        }

    }
}
