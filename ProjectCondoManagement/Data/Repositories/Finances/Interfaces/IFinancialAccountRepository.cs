using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface IAccountRepository : IGenericRepository<FinancialAccount, DataContextFinances>
    {
    }
}
