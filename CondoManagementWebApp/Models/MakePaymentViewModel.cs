using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class MakePaymentViewModel
    {
        public int Id { get; set; }

        public PaymentDto? Payment { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a payment method")]
        public int SelectedPaymentMethodId { get; set; }

        public List<SelectListItem>? PaymentMethods { get; set; } = new List<SelectListItem>();      


        public int SelectedBeneficiaryId { get; set; }


        //fazer validação no controller
        [Display(Name = "Recipient's Omah Wallet Number")]
        public int? BeneficiaryAccountId { get; set; }

        public bool IsPaid { get; set; } = false;


        //parte dos metodos de pagamento 

        // Campos para Cartão de Crédito
        [Display(Name = "Credit card number")]
        public string? CreditCardNumber { get; set; }


        [Display(Name = "Cvv number")]
        public string? Cvv { get; set; }


        [Display(Name = "Expiration date")]
        public string? ExpirationDate { get; set; } // MM/AA


        // Campo para MBWay
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


        public int? PayerFinancialAccountId { get; set; }

        //campo para beneficiário externo
        [Display(Name = "Recipient's Bank Account")]
        public string? ExternalRecipientBankAccount { get; set; }    
    }
}
