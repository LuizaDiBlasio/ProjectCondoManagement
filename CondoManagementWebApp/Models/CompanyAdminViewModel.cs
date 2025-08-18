using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace CondoManagementWebApp.Models
{
    public class CompanyAdminDashboardViewModel
    {
        public IEnumerable<User> Users { get; set; }

        public IEnumerable<Payment> Payments { get; set; }

        public IEnumerable<Fee> Fees { get; set; }

        public IEnumerable<Company> Companies { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
