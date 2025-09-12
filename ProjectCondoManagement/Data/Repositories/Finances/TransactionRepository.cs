using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class TransactionRepository : GenericRepository<Transaction, DataContextFinances>, ITransactionRepository
    {
        private readonly DataContextFinances _contextFinances;
        private readonly IFinancialAccountRepository _financialAccountRepository;
        private readonly IConverterHelper _converterHelper;

        public TransactionRepository(DataContextFinances contextFinances, IFinancialAccountRepository financialAccountRepository, IConverterHelper converterHelper     )
        {
            _contextFinances = contextFinances;
            _financialAccountRepository = financialAccountRepository;
            _converterHelper = converterHelper;
        }

        public DataContextFinances ContextFinances { get; }

        public async Task<IEnumerable<Transaction>> GetTransactions(User user)
        {
            if (user.CompanyId != null)
            {
                return _contextFinances.Transactions.Where(t => t.CompanyId.HasValue && t.CompanyId == user.CompanyId);
            }

            var financialAccount = await _financialAccountRepository.GetByIdAsync(user.FinancialAccountId.Value, _contextFinances);

            return _contextFinances.Transactions.Where(t => t.AccountBeneficiary == financialAccount && t.AccountPayer == financialAccount);

        }

        public async Task<List<Transaction>> GetByFinancialAccountIdAsync(int id)
        {
            return await _contextFinances.Transactions
                .Where(t => t.BeneficiaryAccountId == id || t.PayerAccountId == id)
                .ToListAsync();
        }







    }
}
