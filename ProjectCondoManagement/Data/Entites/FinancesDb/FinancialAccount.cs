using ClassLibrary;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class FinancialAccount :IEntity
    {
        public int Id { get; set; }

        public string OwnerName { get; set; }

        public decimal Balance { get; set; }


        public bool IsActive { get; set; } = false;

        public string? CardNumber { get; set; }  

        public string? AssociatedBankAccount { get; set; }   

        public string? BankName { get; set; }

        [NotMapped]
        public IEnumerable<Transaction>? TransactionsAsPayer { get; set; }
        [NotMapped]
        public IEnumerable<Transaction>? TransactionsAsBeneficiary { get; set; }

    }
}
