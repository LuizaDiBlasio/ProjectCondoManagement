using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class CondoMemberDashboardViewModel
    {

        public List<UnitDto>? UnitDtos { get; set; }

        public FinancialAccountDto? FinancialAccountDto { get; set; }

        public List<MessageDto>? MessageDtos { get; set; }

        public CondoMemberDto? CondoMemberDto { get; set; }

    }
}
