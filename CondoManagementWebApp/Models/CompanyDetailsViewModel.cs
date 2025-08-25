using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class CompanyDetailsViewModel
    {
        public CompanyDto CompanyDto { get; set; }

        public UserDto? CompanyAdmin { get; set; }

        public List<CondominiumDto> CondominiumDtos { get; set; } = new List<CondominiumDto>();
    }
}
