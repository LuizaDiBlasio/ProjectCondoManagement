using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="New password")]
        public string? Password { get; set; } //nova password


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm password")]
        public string? ConfirmPassword { get; set; }


        [Required]
        public string Token { get; set; }

        public string UserId { get; set; }
    }
}
