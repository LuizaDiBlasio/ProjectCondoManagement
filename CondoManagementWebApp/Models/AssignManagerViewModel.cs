using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class AssignManagerViewModel : CondominiumDto
    {

        public IEnumerable<UserDto>? Managers { get; set; }

    }
}
