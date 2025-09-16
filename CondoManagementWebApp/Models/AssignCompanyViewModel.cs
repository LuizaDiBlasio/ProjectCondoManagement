using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoManagementWebApp.Models
{
    public class AssignCompanyViewModel : UserDto
    {
        public string UserRole { get; set; }    

        public int? SelectedCompanyId { get; set; } 

        public List<int>? SelectedCompaniesIds { get; set; } = new List<int>(); 

        public List<SelectListItem>? AvailableCompanies { get; set; }    

    }
}
