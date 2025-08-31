using ClassLibrary;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Transaction : IEntity
    {
        public int Id { get; set; }

        public DateTime DateAndTime { get; set; }

        public int PayerAccountId { get; set; } //FK para TransactionsAsPayer

        public FinancialAccount AccountPayer { get; set; }

        public int? BeneficiaryAccountId { get; set; }// FK para TransactionsAsBeneficiary

        public FinancialAccount? AccountBeneficiary { get; set; }

        public string? ExternalRecipientBankAccount { get; set; }

        public int PaymentId { get; set; }

        public Payment Payment { get; set; }
    }
}
