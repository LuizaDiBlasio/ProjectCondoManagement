using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class ResetPasswordViewModel
    {

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } //nova password


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }


        [Required]
        public string Token {get; set;}

        public string UserId { get; set;}   
    }
}
