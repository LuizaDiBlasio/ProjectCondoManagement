using ClassLibrary.DtoModels;
using Syncfusion.EJ2.Notifications;

namespace CondoManagementWebApp.Models
{
    public class CondoManagerDashboardViewModel
    {
        public UserDto? CondoManager { get; set; }

        public List<OccurrenceDto> Occurrences { get; set; } = new List<OccurrenceDto>();
        public List<CondominiumDto> Condominiums { get; set; } = new List<CondominiumDto>();
        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
        public List<MeetingDto> Meetings { get; set; } = new List<MeetingDto>();




    }
}
