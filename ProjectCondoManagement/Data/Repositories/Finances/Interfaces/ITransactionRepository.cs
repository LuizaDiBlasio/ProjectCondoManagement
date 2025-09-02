using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction, DataContextFinances>
    {
        Task<IEnumerable<Transaction>> GetTransactions(User user);
    }
}
