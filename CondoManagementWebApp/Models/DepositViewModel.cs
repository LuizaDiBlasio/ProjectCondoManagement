using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class DepositViewModel
    {
        public int OwnerId { get; set; }

        [Display(Name ="Amount")]
        public decimal DepositValue { get; set; }

        public int SelectedPaymentMethodId { get; set; } // 1: Bank Transfer, 2: Credit Card, 3: Associated Bank Account

        [Display(Name = "Credit Card Number")]
        public string? CreditCardNumber { get; set; } // For Credit Card

        public int? CompanyId { get; set; } // Optional, in case needed for context


        public string? Cvv { get; set; } // For Credit Card

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; } // For MbWay
    }
}
