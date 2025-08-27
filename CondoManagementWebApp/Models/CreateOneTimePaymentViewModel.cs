using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateOneTimePaymentViewModel
    {
        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }


        [Required]
        public int PayerFinancialAccountId { get; set; }

        public int CondominiumId { get; set; }


        //parte da criação da expense:

        [Required]
        public decimal ExpenseAmount { get; set; }

        [Required]
        public string ExpenseDetail { get; set; }


        [Required]
        public EnumDto ExpenseTypeDto { get; set; }

        public List<SelectListItem> ExpenseTypesList { get; set; }

        public ExpenseDto OneTimeExpense { get; set; }
    }
}
