using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace CondoManagementWebApp.Models
{
    public class FinancialAccountViewModel : FinancialAccount
    {
        public UserDto? User { get; set; }

        public CompanyDto? Company { get; set; }

        public IEnumerable<TransactionDto> Transactions { get; set; }
    }
}
