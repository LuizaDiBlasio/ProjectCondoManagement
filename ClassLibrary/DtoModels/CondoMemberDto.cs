using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class CondoMemberDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public string PhoneNumber { get; set; }

        public string? ImageUrl { get; set; }

        public string? UserId { get; set; }
    }
}
