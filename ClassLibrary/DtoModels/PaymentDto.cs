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

        public DateTime IssueDate { get; set; }

        public DateTime DueDate { get; set; }

        public string PaymentMethod { get; set; }

        public string UserEmail { get; set; }

        public bool IsPaid { get; set; }

        public InvoiceDto? InvoiceDto { get; set; }

        public IEnumerable<ExpenseDto> ExpensesDto { get; set; }

        public decimal TotalAmount => ExpensesDto.Sum(e => e.Amount);

        public TransactionDto? TransactionDto { get; set; }

    }
}
