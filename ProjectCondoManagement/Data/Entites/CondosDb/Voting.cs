using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Voting : IEntity
    {
        public int Id { get; set; }

        public int MeetingId { get; set; }

        public List<Vote> Votes { get; set; }

        public bool Result { get; set; }

        public string Matter { get; set; }
    }
}
