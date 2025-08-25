using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Unit : IEntity
    {
        public int Id { get; set; }

        public Condominium? Condominium { get; set; }

        public int CondominiumId { get; set; }

        public string Floor { get; set; }

        public int Bedrooms { get; set; }

        public IEnumerable<Occurrence>? Occurrences { get; set; }

        public ICollection<CondoMember>? CondoMembers { get; set; }  

        public string Door { get; set; }
    }
}
