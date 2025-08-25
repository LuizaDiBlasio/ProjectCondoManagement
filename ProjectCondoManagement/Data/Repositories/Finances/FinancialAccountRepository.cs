using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class FinancialAccountReposirory : GenericRepository<FinancialAccount, DataContextFinances>, IFinancialAccountRepository
    {
        private readonly DataContextFinances _dataContextFinances;

        public FinancialAccountReposirory(DataContextFinances dataContextFinances)
        {
            _dataContextFinances = dataContextFinances;
        }

        public async Task<FinancialAccount> CreateFinancialAccountAsync()
        {
            var financialAccount = new FinancialAccount()
            {
                InitialDeposit = 0 // depósito inicial vai ser sempre 0
            };

            await CreateAsync(financialAccount, _dataContextFinances); //add FinAcc na Bd

            return financialAccount;    
        }
    }
}
