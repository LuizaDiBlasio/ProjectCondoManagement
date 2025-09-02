using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class DepositViewModel
    {
        public int OwnerId { get; set; }

        public decimal DepositValue { get; set; }

        public int SelectedPaymentMethodId { get; set; } // 1: Bank Transfer, 2: Credit Card, 3: Associated Bank Account

        public string? CreditCardNumber { get; set; } // For Credit Card


        public string? Cvv { get; set; } // For Credit Card


        public string? PhoneNumber { get; set; } // For MbWay
    }
}
