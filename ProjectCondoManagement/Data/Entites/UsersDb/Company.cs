using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class Company : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public string? CompanyAdminId { get; set; }

        [NotMapped]
        public IEnumerable<Condominium>? Condominiums { get; set; }

        public List<int>? CondominiumIds { get; set; }

        public int FinancialAccountId { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string TaxIdDocument { get; set; }
    }
}
