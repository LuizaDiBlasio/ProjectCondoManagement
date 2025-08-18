using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClassLibrary.DtoModels
{
    public class AdminsAndCondosDto
    {
        public List<SelectListItem> Condos { get; set; }
        public List<SelectListItem> Admins { get; set; }
    }
}
