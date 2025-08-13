using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class SysAdminDashboardViewModel
    {
        public IEnumerable<CondoMemberDto> CondoMembers { get; set; }

        public IEnumerable<UserDto> CompanyAdmins { get; set; }

        public IEnumerable<UserDto> CondoManagers { get; set; }

        public SysAdminDashboardViewModel()
        {
            CondoMembers = new List<CondoMemberDto>();
            CompanyAdmins = new List<UserDto>();    
            CondoManagers = new List<UserDto>();    
        }
    }
}
