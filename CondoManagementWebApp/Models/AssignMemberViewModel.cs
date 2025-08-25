using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class AssignMemberViewModel
    {
        public ICollection<CondoMemberDto>? Members { get; set; }

        public int UnitId { get; set; }

        public UnitDto? Unit { get; set; }

        public List<int>? MemberIds { get; set; }
    }
}
