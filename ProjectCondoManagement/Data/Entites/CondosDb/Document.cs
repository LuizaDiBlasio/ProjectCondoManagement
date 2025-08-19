using ClassLibrary;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Document : IEntity
    {
        public int Id { get; set; }

        public int? CondominiumId { get; set; } //nullable pois pode ser um documento global

        public string FileName { get; set; } // Propriedade vinda de IFormFile

        public string ContentType { get; set; } // Propriedade vinda de IFormFile , contente tipo MIME (ex: image/jpeg)

        public string? DocumentUrl { get; set; }

        public string? DocumentFullPath => DocumentUrl == null ?
                    $"https://res.cloudinary.com/ddnkq9dyb/image/upload/v1754230681/noimage_q8mayx.jpg" // caminho relativo ao Url no image
                    : DocumentUrl;

        public DateTime DataUpload { get; set; }

        [NotMapped]
        public IFormFile FileUpload { get; set; }
    }
}
