using ClassLibrary;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Invoice: IEntity
    {
        public int Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public int CondominiumId { get; set; }

        public int PayerAccountId { get; set; }

        public FinancialAccount PayerFinancialAccount { get; set; }

        public int BeneficiaryAccountId { get; set; }

        public FinancialAccount BeneficiaryFinancialAccount { get; set; }

        public int PaymentId { get; set; }  

        public Payment Payment { get; set; }

    }
}
