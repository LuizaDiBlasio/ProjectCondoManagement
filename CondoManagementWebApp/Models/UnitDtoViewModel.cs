using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CondoManagementWebApp.Models
{
    public class UnitDtoViewModel : UnitDto
    {
        public int? CondoId { get; set; }

        public string? CondoName { get; set; }

        public int? MemberId { get; set; }

        public IEnumerable<SelectListItem>? Condos { get; set; }
    }
}
