using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class FinancialAccount :IEntity
    {
        public int Id { get; set; }

        public decimal InitialDeposit { get; set; }

        public IEnumerable<Transaction> TransactionsAsPayer { get; set; }

        public IEnumerable<Transaction> TransactionsAsBeneficiary { get; set; }

    }
}
