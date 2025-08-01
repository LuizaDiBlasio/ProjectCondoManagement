using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class RegisterUserDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public string PhoneNumber { get; set; }

        public string SelectedRole { get; set; }

        public int? CompanyId { get; set; }

        public Guid? ImageId { get; set; }
    }
}
