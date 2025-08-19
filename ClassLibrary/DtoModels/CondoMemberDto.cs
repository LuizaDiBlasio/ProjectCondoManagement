using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class CondoMemberDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Birth date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]  
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string ConduminiumId { get; set; }

        public CondominiumDto CondominiumDto { get; set; }


        public IEnumerable<UnitDto> Units { get; set; }


        public string? ImageUrl { get; set; }

        public string ImageFullPath =>
          string.IsNullOrWhiteSpace(ImageUrl)
          ? "https://res.cloudinary.com/ddnkq9dyb/image/upload/v1754230681/noimage_q8mayx.jpg"
          : ImageUrl;
    }
}
