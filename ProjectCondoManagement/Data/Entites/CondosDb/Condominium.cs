using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class Condominium : IEntity
    {
        public int Id { get; set; }



        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [NotMapped]
        public Company? Company { get; set; }



        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }


        [Display(Name = "Condo Manager")]
        public string ManagerUserId { get; set; }


        [Display(Name = "Condo Name")]
        [Required(ErrorMessage = "Condo name is required.")]
        public string CondoName { get; set; }

        public int? FinancialAccountId { get; set; }

        [NotMapped]
        public User? ManagerUser { get; set; }

        public IEnumerable<Unit> Units { get; set; }

        public IEnumerable<Document> Documents { get; set; }

        public IEnumerable<Meeting> Meetings { get; set; }

        public IEnumerable<Occurrence> Occurences { get; set; }

        public IEnumerable<CondoMember>? CondoMembers { get; set; }
    }
}
