using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class FinancialAccount :IEntity
    {
        public int Id { get; set; }

        public decimal Assets { get; set; }

        public IEnumerable<Transaction> TransactionsAsPayer { get; set; }

        public IEnumerable<Transaction> TransactionsAsBeneficiary { get; set; }

        public decimal Balance { get; set; }

        public string UserId { get; set; }
    }
}
