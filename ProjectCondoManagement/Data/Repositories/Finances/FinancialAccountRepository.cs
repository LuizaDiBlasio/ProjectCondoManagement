using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;
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

        public async Task<FinancialAccount> CreateFinancialAccountAsync(string name)
        {
            var financialAccount = new FinancialAccount()
            {
                OwnerName = name,
                Balance = 0
            };

            await CreateAsync(financialAccount, _dataContextFinances); //add FinAcc na Bd

            return financialAccount;    
        }
    }
}
