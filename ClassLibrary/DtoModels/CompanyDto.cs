using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassLibrary.DtoModels
{
    public class CompanyDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public UserDto? CompanyAdmin { get; set; }

        public string? CompanyAdminId { get; set; }    

        public IEnumerable<CondominiumDto>? CondominiumDtos { get; set; }

        public List<int>? SelectedCondominiumIds { get; set; }    

        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string TaxIdDocument { get; set; }
    }
}
