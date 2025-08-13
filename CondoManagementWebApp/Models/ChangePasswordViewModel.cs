using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class ChangePasswordViewModel
    {

        [Required]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")] //confirma se campo abaixo é igual ao campo de cima.
        public string Confirm { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
