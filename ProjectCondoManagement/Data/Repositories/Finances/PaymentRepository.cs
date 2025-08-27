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
                                .FirstOrDefaultAsync(p => p.Id == id);     
        }

        public List<SelectListItem> GetPaymentMethodsList()
        {
            var selectList = new List<SelectListItem>
            {
                new SelectListItem{Value = "0", Text = "Select a shift..."},
                new SelectListItem{Value = "1", Text = "MbWay"},
                new SelectListItem{Value = "2", Text = "Credit card"},
                new SelectListItem{Value = "3", Text = "Apple pay"}
            };
            return selectList;
        }
    }
}
