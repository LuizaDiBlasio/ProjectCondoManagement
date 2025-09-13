using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class CompanyAdminDashboardViewModel
    {
        public UserDto CompanyAdmin { get; set; } 

        public FinancialAccountDto FinancialAccount { get; set; }

        public List<CondominiumDto> Condominiums { get; set; } = new List<CondominiumDto>();

        public List<UserDto> CondoManagers { get; set; } = new List<UserDto>();

        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();

        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();






    }
}
