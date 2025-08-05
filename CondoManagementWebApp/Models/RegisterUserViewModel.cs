using CondoManagementWebApp.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "You need to register a full name")]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters length")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Required(ErrorMessage = "You need to register an email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required(ErrorMessage = "You need to register an address")]
        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters length")]
        public string Address { get; set; }


        [Required(ErrorMessage = "The Date of Birth is required.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }


        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "You need to register a phone number")]
        [MaxLength(30, ErrorMessage = "The field {0} can only contain {1} characters length")]
        public string PhoneNumber { get; set; }



        [Required(ErrorMessage = "You need to select a role for your user")]
        [Display(Name = "User role")]
        public string SelectedRole { get; set; }


        public IEnumerable<SelectListItem> AvailableRoles { get; set; }


        [Display(Name = "Company")]
        public int SelectedCompanyId { get; set; }

        public IEnumerable<SelectListItem> Companies { get; set; }

        [Display(Name = "Profile picture")]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }

        public string? ImageUrl { get; set; }
    }
}
