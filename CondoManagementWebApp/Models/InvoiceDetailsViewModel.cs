using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class InvoiceDetailsViewModel
    {
        public int Id { get; set; } 

        public DateTime PaymentDate { get; set; }


        public FinancialAccountDto? PayerFinancialAccountDto { get; set; }


        public FinancialAccountDto? BeneficiaryFinancialAccountDto { get; set; }


        public PaymentDto Payment { get; set; }

        public int CondominiumFinancialAccountId { get; set; } 

    }
}
