using Microsoft.AspNetCore.Identity;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber {  get; set; }    

        public DateTime? BirthDate { get; set; }

        public int? CompanyId { get; set; }

        public Company? Company { get; set; }

        public bool IsActive { get; set; }  = true; 

        public bool Uses2FA {  get; set; }  //TODO: Tirar essa propriedade quando publicar

        public string? ImageUrl { get; set; }
    
        public string? ImageFullPath => ImageUrl == string.Empty? 
                     $"https://res.cloudinary.com/ddnkq9dyb/image/upload/v1754230681/noimage_q8mayx.jpg" // caminho relativo ao Url no image
                     : ImageUrl;
    }
}
