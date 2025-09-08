using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateMeetingViewModel
    {
        public int Id { get; set; }


        [Required]
        public DateTime? DateAndTime { get; set; }

        //public DocumentDto? Report { get; set; }


        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public List<SelectListItem>? CondoMembersToSelect { get; set; }

        [Required]
        public List<int> SelectedCondoMembersIds { get; set; }


        public List<SelectListItem>? CondosToSelect { get; set; }

        [Required]
        public int? CondominiumId { get; set; }

        public List<SelectListItem>? OccurrencesToSelect { get; set; }

        [Required]
        public List<int> SelectedOccurrencesIds { get; set; }

        public bool IsExtraMeeting { get; set; } = false; // (regular = false/ extra = true)

    }
}
