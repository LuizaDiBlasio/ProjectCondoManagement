using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class MakePaymentViewModel
    {
        public int Id { get; set; } 

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a payment method")]
        public int SelectedPaymentMethodId { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();

        [Required]
        public int BeneficiaryAccountId { get; set; }   

        public bool IsPaid { get; set; } = false;

    }
}
