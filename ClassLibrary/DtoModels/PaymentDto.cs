using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public string? PaymentMethod { get; set; }

        public int PayerFinancialAccountId { get; set; }

        public int CondominiumId { get; set; }

        public bool IsPaid { get; set; } = false;

        public int? InvoiceId { get; set; }  

        public InvoiceDto? InvoiceDto { get; set; }

        public IEnumerable<ExpenseDto> ExpensesDto { get; set; } = new List<ExpenseDto>();  

        public ExpenseDto? OneTimeExpenseDto { get; set; }

        public decimal TotalAmount => ExpensesDto.Sum(e => e.Amount);

        public int? TransactionId { get; set; }

        public TransactionDto? TransactionDto { get; set; }

        public string? MbwayNumber { get; set; }

        public string? CreditCard { get; set; }


    }
}
