using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class EditOccurrenceViewModel
    {
        public int Id { get; set; }

        public List<SelectListItem>? CondosToSelect { get; set; }

        [Required]
        [Display(Name = "Occurrence Condominium")]
        public int? CondominiumId { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Details { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime? DateAndTime { get; set; }


        [Display(Name = "Units involved")]
        public List<int>? SelectedUnitIds { get; set; }


        public List<SelectListItem>? UnitsToSelect { get; set; }

        [Display(Name = "Status")]
        public bool IsResolved { get; set; } = false;
    }
}
