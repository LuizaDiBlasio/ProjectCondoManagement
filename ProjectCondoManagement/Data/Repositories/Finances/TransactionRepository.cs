using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class TransactionRepository : GenericRepository<Transaction, DataContextFinances>, ITransactionRepository
    {
    }
}
