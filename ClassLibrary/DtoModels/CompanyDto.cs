using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class CompanyDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<UserDto> Users { get; set; }

        public IEnumerable<CondominiumDto>? Condominiums { get; set; }

        public string Email { get; set; }

        public string Addres { get; set; }

        public string PhoneNumber { get; set; }

        public string TaxIdDocument { get; set; }
    }
}
