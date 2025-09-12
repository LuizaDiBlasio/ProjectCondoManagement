using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class AssignCompanyViewModel : UserDto
    {
        public List<CompanyDto> Companies { get; set; } = new List<CompanyDto>();

    }
}
