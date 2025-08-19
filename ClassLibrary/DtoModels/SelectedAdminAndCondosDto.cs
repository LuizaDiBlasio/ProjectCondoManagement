using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class SelectedAdminAndCondosDto
    {
        public List<CondominiumDto> SelectedCondos { get; set; } = new List<CondominiumDto>();

        public UserDto? SelectedAdmin { get; set; }
    }
}
