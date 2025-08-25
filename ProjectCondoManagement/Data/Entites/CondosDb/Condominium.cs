using ClassLibrary;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Condominium : IEntity
    {
        public int Id { get; set; }

        public int? CompanyId { get; set; }

        [NotMapped]
        public Company? Company { get; set; }

        public string Address { get; set; }

        public string CondoName { get; set; }

        public string? ManagerUserId { get; set; }

        [NotMapped]
        public User? ManagerUser { get; set; }

        public IEnumerable<Unit>? Units { get; set; }

        public IEnumerable<Document>? Documents { get; set; }

        public IEnumerable<Meeting>? Meetings { get; set; }


        public IEnumerable<Occurrence>? Occurrences { get; set; }



    }
}
