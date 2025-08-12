using CondoManagementWebApp.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Full Name")]
        [MaxLength(50, ErrorMessage = "The field {0} allows only {1} characters")] //mensagem não chega a ser mostrada
        public string FullName { get; set; }



        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }


        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }


        [Required]
        public string Address { get; set; }


        public string? Email { get; set; }


        [Display(Name = "Profile Image")]
        [MaxFileSize(5 * 1024 * 1024)]
        public IFormFile ImageFile { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImageFullPath => ImageUrl == null ?
                     $"https://res.cloudinary.com/ddnkq9dyb/image/upload/v1754230681/noimage_q8mayx.jpg" // caminho relativo ao Url no image
                     : ImageUrl;
    }
}
