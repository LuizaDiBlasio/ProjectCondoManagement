using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class CondominiumRepository : GenericRepository<Condominium, DataContextCondos>, ICondominiumRepository
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly DataContextCondos _dataContextCondos;
        private readonly DataContextFinances _dataContextFinances;
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public CondominiumRepository(IUserHelper userHelper, IConverterHelper converterHelper, DataContextCondos dataContextCondos, DataContextFinances dataContextFinances, IFinancialAccountRepository financialAccountRepository)
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _dataContextCondos = dataContextCondos;
            _dataContextFinances = dataContextFinances;
            _financialAccountRepository = financialAccountRepository;
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



        public async Task<Response<object>> LinkManager(List<CondominiumDto> condominiums)
        {
            try
            {
                IEnumerable<User> managers = await _userHelper.GetUsersByRoleAsync("CondoManager");
                if (managers == null || !managers.Any())
                {
                    return new Response<object>
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

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Managers linked successfully"
                };
            }
            catch (Exception ex)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Error linking manager: {ex.Message}"
                };
            }


        }

        public async Task<Response<object>> LinkFinancialAccount(List<CondominiumDto> condominiums)
        {
            try
            {
                IEnumerable<FinancialAccount> accounts = await _financialAccountRepository.GetAll(_dataContextFinances).ToListAsync();
                if (accounts == null || !accounts.Any())
                {
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = "No accounts found"
                    };
                }


                foreach (CondominiumDto condominium in condominiums)
                {
                    var account = accounts.FirstOrDefault(m => m.Id == condominium.FinancialAccountId);
                    if (account != null)
                    {
                        var accountDto = _converterHelper.ToFinancialAccountDto(account, false);

                        condominium.FinancialAccountDto = accountDto;
                    }
                }

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Accounts linked successfully"
                };
            }
            catch (Exception ex)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Error linking Account: {ex.Message}"
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

        public async Task<List<CondoMember>> GetCondoCondomembers(int condoId)
        {
            return _dataContextCondos.Units
                    .Where(u => u.CondominiumId == condoId)
                    .SelectMany(u => u.CondoMembers!)
                    .ToList();

        }

        public async Task<Response<object>> LinkFinancialAccountWithTransactions(List<CondominiumDto> condominiums)
        {
            try
            {
                IEnumerable<FinancialAccount> accounts = await _financialAccountRepository
                                                                .GetAll(_dataContextFinances)
                                                                .Include(fa => fa.TransactionsAsPayer)
                                                                .Include(fa => fa.TransactionsAsBeneficiary)
                                                                .ToListAsync();

                if (accounts == null || !accounts.Any())
                {
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = "No accounts found"
                    };
                }


                foreach (CondominiumDto condominium in condominiums)
                {
                    var account = accounts.FirstOrDefault(m => m.Id == condominium.FinancialAccountId);
                    if (account != null)
                    {
                        var accountDto = _converterHelper.ToFinancialAccountDto(account, false);

                        condominium.FinancialAccountDto = accountDto;
                    }
                }

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Accounts linked successfully"
                };
            }
            catch (Exception ex)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Error linking Account: {ex.Message}"
                };
            }
        }

        public Task<IEnumerable<CondominiumDto>?> GetCondominiumsByCompanyIdAsync(int id)
        {
            var condos = _dataContextCondos.Condominiums
                            .Where(c => c.CompanyId == id).Include(c => c.Units).Include(c => c.Occurrences).Include(c => c.Meetings);

            var condosDto = condos.Select(c => _converterHelper.ToCondominiumDto(c, false));

            return  condosDto.Any() ? Task.FromResult<IEnumerable<CondominiumDto>?>(condosDto) : Task.FromResult<IEnumerable<CondominiumDto>?>(null);

        }

        public Task<Condominium> GetCondominiumByFinancialAccountIdAsync(int id)
        {
            return _dataContextCondos.Condominiums.FirstOrDefaultAsync(c => c.FinancialAccountId == id);
        }
    }
}
