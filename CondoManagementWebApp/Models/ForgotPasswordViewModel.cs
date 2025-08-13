using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }   
    }
}
