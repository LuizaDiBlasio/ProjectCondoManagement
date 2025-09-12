using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class MyCondosViewModel
    {
       public IEnumerable<CondominiumDto> MyCondos { get; set; } = new List<CondominiumDto>();
    }
}
