using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Payment : IEntity
    {
        public int Id { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public string? PaymentMethod { get; set; }

        public int PayerFinancialAccountId { get; set; } 

        public int CondominiumId {  get; set; }

        public bool IsPaid { get; set; } = false;

        public int? InvoiceId { get; set; } 

        public Invoice? Invoice { get; set; }

        public List<Expense> Expenses { get; set; } = new List<Expense>();   

        public Expense? OneTimeExpense { get; set; }

        public decimal TotalAmount => Expenses.Sum(e => e.Amount);

        public int? TransactionId { get; set; }  

        public Transaction? Transaction { get; set; }

       

    }
}
