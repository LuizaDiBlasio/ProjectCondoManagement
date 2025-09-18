using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class CondoMemberDashboardViewModel
    {

        public List<UnitDto>? UnitDtos { get; set; }

        public FinancialAccountDto? FinancialAccountDto { get; set; }

        public List<MessageDto>? MessageDtos { get; set; }

        public CondoMemberDto? CondoMemberDto { get; set; }


        public List<OccurrenceDto>? OccurrenceDtos { get; set; }


        public List<PaymentDto>? PaymentsDtos { get; set; }
        public List<MeetingDto>? MeetingsDtos { get; internal set; }
    }
}
