using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Helpers
{
    public class FinancialAccountHelper : IFinancialAccountHelper
    {
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly DataContextFinances _dataContextFinances;

        public FinancialAccountHelper(IFinancialAccountRepository financialAccountRepository, DataContextFinances dataContextFinances)
        {
            _financialAccountRepository = financialAccountRepository;
            _dataContextFinances = dataContextFinances;
        }
        public async Task UpdateFinancialAccountNameAsync(int accountId, string ownerName)
        {
            var financialAccount = await  _financialAccountRepository.GetByIdAsync(accountId, _dataContextFinances);

            if (financialAccount.OwnerName != ownerName)
            {
                financialAccount.OwnerName = ownerName;
                await _financialAccountRepository.UpdateAsync(financialAccount, _dataContextFinances);
            }
        }



    }
}
