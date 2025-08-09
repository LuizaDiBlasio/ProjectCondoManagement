using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class LoginViewModel
    {
        [Required]//obrigatorio
        [EmailAddress] //formato email
        public string Username { get; set; }

        [Required]
        [MinLength(6)] //minimo de caracteres na validação
        public string Password { get; set; }

        public bool Requires2FA { get; set; }
    }
}
