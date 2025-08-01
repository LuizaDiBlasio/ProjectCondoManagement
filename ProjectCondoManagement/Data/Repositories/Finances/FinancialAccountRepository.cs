using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class AccountReposirory : GenericRepository<FinancialAccount, DataContextFinances>, IAccountRepository
    {
    }
}
