using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateMeetingViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display (Name ="Data and time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DateAndTime { get; set; }

        //public DocumentDto? Report { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "Subject")]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        public List<SelectListItem>? CondoMembersToSelect { get; set; }

        [Required]
        [Display(Name = "Selected Condo Members")]
        public List<int> SelectedCondoMembersIds { get; set; }


        public List<SelectListItem>? CondosToSelect { get; set; }

        [Required]
        [Display(Name = "Selected Condominium")]
        public int? CondominiumId { get; set; }

        public List<SelectListItem>? OccurrencesToSelect { get; set; }

       
        [Display(Name = "Selected Occurrences")]
        public List<int>? SelectedOccurrencesIds { get; set; } = new List<int>();

        public bool IsExtraMeeting { get; set; } = false; // (regular = false/ extra = true)

    }
}
