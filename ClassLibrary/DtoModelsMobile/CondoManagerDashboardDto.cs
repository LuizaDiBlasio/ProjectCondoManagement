using ClassLibrary.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModelsMobile
{
    public class CondoManagerDashboardDto
    {
        public UserDto? CondoManager { get; set; }

        public List<OccurrenceDto> Occurrences { get; set; } = new List<OccurrenceDto>();
        public List<CondominiumDto> Condominiums { get; set; } = new List<CondominiumDto>();
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public List<MeetingDto> Meetings { get; set; } = new List<MeetingDto>();

    }
}
