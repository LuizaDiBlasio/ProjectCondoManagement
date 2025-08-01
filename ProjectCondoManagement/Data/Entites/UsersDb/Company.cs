using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class Company : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<User> Users { get; set; }

        public IEnumerable<CompanyCondominium> CompanyCondominiums { get; set; }

        public string Email { get; set; }

        public string Addres { get; set; }

        public string PhoneNumber { get; set; }

        public string TaxIdDocument { get; set; }
    }
}
