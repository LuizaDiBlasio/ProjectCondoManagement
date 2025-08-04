using Microsoft.AspNetCore.Identity;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public string Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? CompanyId { get; set; }

        public Company? Company { get; set; }

        public string? ImageUrl { get; set; }

        //TODO Tratar do blob (está com endereço blob antigo)
     //   public string ImageFullPath => ImageId == null || ImageId == Guid.Empty
     //? $"/imagens/noImage.jpg" // caminho relativo à raiz da aplicação!
     //: $"https://gestaoescolar.blob.core.windows.net/imagens/{ImageId}";
    }
}
