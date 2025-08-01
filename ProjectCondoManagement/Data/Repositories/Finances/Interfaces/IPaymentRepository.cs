using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.FinancesDb;

namespace ProjectCondoManagementAPI.Data.Repositories.Finances.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment, DataContextFinances>
    {
    }
}
