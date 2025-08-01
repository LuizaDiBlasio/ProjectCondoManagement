using ClassLibrary;

namespace ProjectCondoManagementAPI.Data.Entites.CondosDb
{
    public class Vote : IEntity
    {
        public int Id { get; set; }

        public bool YesNoVote { get; set; }

        public int CondoMemberId { get; set; }

        public CondoMember CondoMember { get; set; }
    }
}
