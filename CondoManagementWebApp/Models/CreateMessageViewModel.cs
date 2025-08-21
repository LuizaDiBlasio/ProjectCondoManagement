using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateMessageViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} field must be at most {1} characters long.")]
        [Display(Name = "Title")]
        public string MessageTitle { get; set; }


        [Required]
        [StringLength(400, ErrorMessage = "The {0} field must be at most {1} characters long.")]
        [Display(Name = "Message")]
        public string MessageContent { get; set; }


        [Required]
        [Display(Name = "From")]
        public string ReceiverEmail { get; set; }

        public bool DeletedBySender { get; set; } = false;

        public bool DeletedByReceiver { get; set; } = false;
    }
}
