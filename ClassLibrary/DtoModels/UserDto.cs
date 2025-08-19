using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ClassLibrary.DtoModels
{
    public class UserDto
    {
        public string Id { get; set; }    

        public string FullName { get; set; } 

        public string Address { get; set; } 

        public string PhoneNumber { get; set; } 

        public string Email { get; set; }   

        public DateTime? BirthDate { get; set; } 

        public int? CompanyId { get; set; } 

        public int? FinancialAccountId { get; set; } 

        public bool IsActive { get; set; } = true;

        public bool Uses2FA { get; set; }   //TODO: Tirar essa propriedade quando publicar

        public string? ImageUrl { get; set; }

        public string? ImageFullPath => ImageUrl == null ?
                    $"https://res.cloudinary.com/ddnkq9dyb/image/upload/v1754230681/noimage_q8mayx.jpg" // caminho relativo ao Url no image
                    : ImageUrl;

    }
}
