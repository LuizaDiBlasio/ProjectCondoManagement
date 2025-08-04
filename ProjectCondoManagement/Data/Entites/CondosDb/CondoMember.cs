using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class CondoMember : IEntity
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string? IdDocument { get; set; }

        public string? TaxIdNumber { get; set; }

        public IEnumerable<Meeting>? MeetingsAttended { get; set; }

        public string? UserId { get; set; }

        public string? ImageUrl { get; set; }




        //TODO Tratar do blob (está com endereço bloc antigo)
     //   public string ImageFullPath => ImageUrl == null || ImageUrl == string.Empty
     //? $"/imagens/noImage.jpg" // caminho relativo à raiz da aplicação!
     //: $"https://gestaoescolar.blob.core.windows.net/imagens/{ImageUrl}";
    }
}

