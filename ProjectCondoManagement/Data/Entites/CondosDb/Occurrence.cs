using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Occurrence : IEntity
    {
        public int Id { get; set; }

        public string Details { get; set; }

        public DateTime DateAndTime { get; set; }

        public DateTime ResolutionDate { get; set; }

        public IEnumerable<Unit> Units { get; set; }

        public int? Meeting { get; set; }  //nullable para marcar se foi ou não discutida em uma reunião
    }
}
