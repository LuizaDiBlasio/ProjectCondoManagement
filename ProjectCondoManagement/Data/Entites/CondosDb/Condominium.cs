using ClassLibrary;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.Reflection.Metadata;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Condominium : IEntity
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public string Address { get; set; }

        public string ManagerUserId { get; set; }

        public IEnumerable<Unit> Units { get; set; }

        public IEnumerable<Document> Documents { get; set; }

        public IEnumerable<Meeting> Meetings { get; set; }

        public IEnumerable<Occurrence> Occurences { get; set; }
    }
}
