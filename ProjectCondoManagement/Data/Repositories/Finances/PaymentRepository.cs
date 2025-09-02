using ClassLibrary;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class PaymentRepository : GenericRepository<Payment, DataContextFinances>, IPaymentRepository
    {
        private readonly DataContextFinances _dataContextFinances;

        public PaymentRepository(DataContextFinances dataContextFinances)
        {
            _dataContextFinances = dataContextFinances;
        }

        public async Task<Payment?> GetPaymentWithExpenses(int id)
        {
                return await _dataContextFinances.Payments
                                .Include(p => p.Expenses)
                                .Include(p => p.OneTimeExpense)
                                .FirstOrDefaultAsync(p => p.Id == id);     
        }

       
    }
}
