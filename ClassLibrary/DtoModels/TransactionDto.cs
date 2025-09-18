using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DtoModels
{
    public class TransactionDto
    {
        public int Id { get; set; }

        public DateTime DateAndTime { get; set; }


        public int? PayerAccountId { get; set; } //FK para TransactionsAsPayer

        public FinancialAccountDto? AccountPayerDto { get; set; }

        public int? BeneficiaryAccountId { get; set; }// FK para TransactionsAsBeneficiary

        public FinancialAccountDto? AccountBeneficiaryDto { get; set; }

        public decimal? Amount { get; set; }

        public string? ExternalRecipientBankAccount { get; set; }

        public string? RecipientName { get; set; }


        public int? PaymentId { get; set; }

        public int? CompanyId { get; set; }  

    }
}