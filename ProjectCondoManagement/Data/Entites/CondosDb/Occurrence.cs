using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Occurrence : IEntity
    {
        public int Id { get; set; }

        public string Details { get; set; }

        public string Subject { get; set; }

        public DateTime DateAndTime { get; set; }

        public DateTime? ResolutionDate { get; set; }

        public List<Unit> Units { get; set; }

        public bool IsResolved { get; set; } = false;

        public int CondominiumId { get; set; }  
    }
}
