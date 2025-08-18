using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface IFeeRepository : IGenericRepository<Fee,DataContextFinances>
    {
    }
}
