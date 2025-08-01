using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.FinancesDb;
using ProjectCondoManagementAPI.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagementAPI.Data.Repositories.Finances
{
    public class AccountReposirory : GenericRepository<FinancialAccount, DataContextFinances>, IAccountRepository
    {
    }
}
