using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateRecurringPaymentViewModel
    {
        [Required]  
        public DateTime IssueDate { get; set; }

        [Required]  
        public DateTime DueDate { get; set; }

        [Required]
        public int PayerFinancialAccountId { get; set; }


        public int CondominiumId { get; set; }


        [Required]  
        public IEnumerable<ExpenseDto> ExpensesDto { get; set; }

        public List<SelectListItem> ExpensesToSelect { get; set; } = new List<SelectListItem>();
    }
}
