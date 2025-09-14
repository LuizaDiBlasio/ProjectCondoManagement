using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoManagementWebApp.Models
{
    public class AssignCompanyViewModel : UserDto
    {
        public List<CompanyDto> Companies { get; set; } = new List<CompanyDto>();

        public string UserRole { get; set; }    

        public int? SelectedCompanyId { get; set; } 

        public List<int>? SelectedCompaniesIds { get; set; }

        public List<SelectListItem> AvailableCompanies { get; set; }    

    }
}
