using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class PaymentDetailsViewModel
    {
        public int CondominiumFinancialAccountId { get; set; }  

        public PaymentDto PaymentDto { get; set; }
    }
}
