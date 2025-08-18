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

        public IEnumerable<Unit> Units { get; set; }

        public string? ImageUrl { get; set; }
  
        public string? ImageFullPath => ImageUrl == string.Empty ?
                     $"https://res.cloudinary.com/ddnkq9dyb/image/upload/v1754230681/noimage_q8mayx.jpg" // caminho relativo ao Url no image
                     : ImageUrl;
    }
}

