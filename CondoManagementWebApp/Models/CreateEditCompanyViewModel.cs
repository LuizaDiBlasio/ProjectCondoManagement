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


       
        [Display(Name="Company Admin")]
        public string? SelectedCompanyAdminId { get; set; }  

        public IEnumerable<SelectListItem>? CompanyAdminsToSelect { get; set; }


     
        [Display (Name="Select Company Condominiums")]
        public List<int>? SelectedCondominiumIds { get; set; }  
        
        public IEnumerable<SelectListItem>? CondominiumsToSelect { get; set; }


        [Required]
        public string Email { get; set; }


        [Required]
        public string Address { get; set; }


        [Required]
        [Display(Name="Phone Number")]
        public string PhoneNumber { get; set; }


        [Required]
        [Display(Name="Tx Id Number")]
        public string TaxIdDocument { get; set; }
    }
}
