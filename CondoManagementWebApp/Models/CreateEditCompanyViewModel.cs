using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateEditCompanyViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }


        [Required]
        public string Address { get; set; }

        [Required]
        public int FinancialAccountId { get; set; }


        [Required]
        [Display(Name="Phone Number")]
        public string PhoneNumber { get; set; }


        [Required]
        [Display(Name="Tx Id Number")]
        public string TaxIdDocument { get; set; }
    }
}
