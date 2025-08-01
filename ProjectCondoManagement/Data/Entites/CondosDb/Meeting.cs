using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Meeting :IEntity
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }

        public Condominium Condominium { get; set; }

        public DateTime DateAndTime { get; set; }

        public Document Report { get; set; }

        public IEnumerable<CondoMember> CondoMembers { get; set; }

        public IEnumerable<Voting> Votings { get; set; }

        public IEnumerable<Occurrence> Occurences { get; set; }

        public bool MeetingType { get; set; }  // (regular/extra)
    }
}
