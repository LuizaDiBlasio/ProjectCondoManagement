using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class FinancialAccountDto
    {
        public int Id { get; set; }

        public decimal Deposit { get; set; }

        public decimal Balance { get; set; }    

        public bool IsActive { get; set; } = false;

        public string? CardNumber { get; set; }

        public string? AssociatedBankAccount { get; set; }

        public string? BankName { get; set; }

        public IEnumerable<TransactionDto>? TransactionsAsPayerDto { get; set; }

        public IEnumerable<TransactionDto>? TransactionsAsBeneficiaryDto { get; set; }
    }
}
