using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class EditUserDetailsDto : UserDto
    {

        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }


        public string? Email { get; set; }


        public bool IsActive { get; set; }


        public string? ImageUrl { get; set; }

        public string? CompanyName { get; set; } 

        public int? CompanyId { get; set; }

        public int? FinancialAccountId { get; set; }  

    }
}
