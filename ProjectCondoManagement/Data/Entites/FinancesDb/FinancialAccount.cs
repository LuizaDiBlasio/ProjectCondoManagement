using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class FinancialAccount :IEntity
    {
        public int Id { get; set; }

        public decimal InitialDeposit { get; set; }

        public bool IsActive { get; set; } = false;

        public string? CardNumber { get; set; }

        public string? AssociatedBankAccount { get; set; }

        public string? BankName { get; set; }

        public decimal Balance { get; set; }

        public IEnumerable<Transaction>? TransactionsAsPayer { get; set; }

        public IEnumerable<Transaction>? TransactionsAsBeneficiary { get; set; }
    }
}
