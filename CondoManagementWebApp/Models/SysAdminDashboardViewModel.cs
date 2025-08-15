using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class SysAdminDashboardViewModel
    {
        public IEnumerable<UserDto> CondoMembers { get; set; }

        public IEnumerable<UserDto> CompanyAdmins { get; set; }

        public IEnumerable<UserDto> CondoManagers { get; set; }

        public string Email { get; set; }   

        public SysAdminDashboardViewModel()
        {
            CondoMembers = new List<UserDto>();
            CompanyAdmins = new List<UserDto>();    
            CondoManagers = new List<UserDto>();    
        }
    }
}
