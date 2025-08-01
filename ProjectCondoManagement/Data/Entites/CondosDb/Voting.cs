using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Voting : IEntity
    {
        public int Id { get; set; }

        public int MeetingId { get; set; }

        public Meeting Meeting { get; set; }

        public IEnumerable<Vote> Votes { get; set; }

        public bool Result { get; set; }

        public string Matter { get; set; }
    }
}
