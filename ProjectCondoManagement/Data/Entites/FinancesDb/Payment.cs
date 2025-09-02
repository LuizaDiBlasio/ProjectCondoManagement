using ClassLibrary;
using ClassLibrary.DtoModels;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Payment : IEntity
    {
        public int Id { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public string PaymentMethod { get; set; }

        public string UserId { get; set; }

        public bool IsPaid { get; set; }

        public Invoice? Invoice { get; set; }

        public IEnumerable<Expense> Expenses { get; set; }

        public Transaction? Transaction { get; set; }

        public Expense? OneTimeExpense { get; set; }

        public int CondominiumId { get; set; }
    }
}
