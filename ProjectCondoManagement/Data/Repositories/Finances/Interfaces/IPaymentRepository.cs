using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using System.Web.Mvc;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment, DataContextFinances>
    {
        Task<Payment?> GetPaymentWithExpenses(int id);

    }
}
