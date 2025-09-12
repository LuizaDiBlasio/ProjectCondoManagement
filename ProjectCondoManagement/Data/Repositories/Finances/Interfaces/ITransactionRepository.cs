using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction, DataContextFinances>
    {
        Task<IEnumerable<Transaction>> GetTransactions(User user);

        Task<List<Transaction>> GetByFinancialAccountIdAsync(int id);
    }
}
