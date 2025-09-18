using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class EditMeetingViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Condominium")]
        public int? CondominiumId { get; set; }

        [Required]
        [Display (Name ="Date and time")]
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
        [Display(Name = "Selected condo members")]
        public List<int> SelectedCondoMembersIds { get; set; }


        public List<SelectListItem>? CondosToSelect { get; set; }


        //public VotingDto? Voting { get; set; }

        //public int? VotingId { get; set; }

        public List<SelectListItem>? OccurrencesToSelect { get; set; }

     
        [Display(Name = "Selected occurrences")]
        public List<int>? SelectedOccurrencesIds { get; set; } = new List<int>();

        public bool MeetingType { get; set; } = false; // (regular = false/ extra = true)
    }
}
