using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class SysAdminDashboardViewModel
    {
        public IEnumerable<UserDto> CondoMembers { get; set; }

        public IEnumerable<UserDto> CompanyAdmins { get; set; }

        public IEnumerable<UserDto> CondoManagers { get; set; }

        public string Email { get; set; }

        // PROPRIEDADES PARA A BUSCA
        public string? SearchTerm { get; set; }
        public List<UserDto>? HomonymUsers { get; set; }
        public bool HasHomonyms => HomonymUsers != null && HomonymUsers.Count > 1; //true se houver mais de um user com o mesmo nome

        public SysAdminDashboardViewModel()
        {
            CondoMembers = new List<UserDto>();
            CompanyAdmins = new List<UserDto>();    
            CondoManagers = new List<UserDto>();    
        }
    }
}
