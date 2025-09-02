using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.Security.Principal;

namespace CondoManagementWebApp.Models
{
    public class CompanyAdminDashboardViewModel
    {
        public IEnumerable<UserDto> Users { get; set; }

        public IEnumerable<Payment> Payments { get; set; }

        public IEnumerable<FeeDto> Fees { get; set; }

        public IEnumerable<CondominiumDto> Condominiums { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
