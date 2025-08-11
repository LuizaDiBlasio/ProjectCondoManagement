using ClassLibrary;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Invoice: IEntity
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public int CondominiumId { get; set; }

        public int AccountId { get; set; }

        public FinancialAccount FinancialAccount { get; set; }

        public string UserId { get; set; }

        public Payment Payment { get; set; }

    }
}
