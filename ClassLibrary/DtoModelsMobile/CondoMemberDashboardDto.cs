using ClassLibrary.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModelsMobile
{
    public class CondoMemberDashboardDto
    {
        public CondoMemberDto CondoMember { get; set; }
        public List<UnitDto> Units { get; set; } = new();
        public List<OccurrenceDto> Occurrences { get; set; } = new();
        public FinancialAccountDto FinancialAccount { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();
        public List<MeetingDto> Meetings { get; set; } = new();
        public List<MessageDto> Messages { get; set; } = new();
    }
}
