using ClassLibrary.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModelsMobile
{
    public class CompanyAdminDashboardDto
    {
        public UserDto CompanyAdmin { get; set; }

        public FinancialAccountDto FinancialAccount { get; set; }

        public List<CondominiumDto> Condominiums { get; set; } = new List<CondominiumDto>();

        public List<UserDto> CondoManagers { get; set; } = new List<UserDto>();

        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();

        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
