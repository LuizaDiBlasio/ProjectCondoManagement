using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class TransactionRepository : GenericRepository<Transaction, DataContextFinances>, ITransactionRepository
    {
        private readonly DataContextFinances _contextFinances;
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public TransactionRepository(DataContextFinances contextFinances,IFinancialAccountRepository financialAccountRepository)
        {
            _contextFinances = contextFinances;
            _financialAccountRepository = financialAccountRepository;
        }

        public async Task<IQueryable<Transaction>> GetTransactions(User user)
        {
            if(user.CompanyId != null)
            {
                return _contextFinances.Transactions.Where(t => t.CompanyId == user.CompanyId)
                .Include(t => t.AccountBeneficiary)
                .Include(t => t.AccountPayer);
                
            }

            var financialAccount = await _financialAccountRepository.GetByIdAsync(user.FinancialAccountId.Value,_contextFinances);

            return _contextFinances.Transactions.Where(t => t.AccountBeneficiary == financialAccount && t.AccountPayer == financialAccount)
            .Include(t => t.AccountBeneficiary)
            .Include(t => t.AccountPayer); 

        }
    }
}
