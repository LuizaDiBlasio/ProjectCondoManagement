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

        public Guid BlobId { get; set; }

        //TODO Tratar do bloc
        public string BlobUrl => BlobId == Guid.Empty
       ? $"https://gestaoescolar.blob.core.windows.net/imagens/noImage.jpeg"
         : $"https://gestaoescolar.blob.core.windows.net/imagens/{BlobId}";

        public DateTime DataUpload { get; set; }

        [NotMapped]
        public IFormFile FileUpload { get; set; }
    }
}
