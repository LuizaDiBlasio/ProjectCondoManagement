using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Meeting :IEntity
    {
        public int Id { get; set; }

        public int CondominiumId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateAndTime { get; set; }

        public Document? Report { get; set; }

        public List<CondoMember> CondoMembers { get; set; }

        public List<Occurrence> Occurences { get; set; }

        public bool MeetingType { get; set; }  // (regular = false/ extra = true)

        public string MeetingLink { get; set; } 
    }
}
