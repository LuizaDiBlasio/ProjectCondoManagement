using ClassLibrary.DtoModels;
using CondoManagementWebApp.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateOneTimePaymentViewModel
    {

        [DateGreaterThanToday(ErrorMessage = "Issue Date must be a future date.")]
        [Required(ErrorMessage = "The due date is required.")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }


        [Required]
        [Display(Name = "Omah wallet number")]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than zero.")]
        public int PayerFinancialAccountId { get; set; }

        public List<SelectListItem>? CondosToSelect { get; set; }

        [Required]
        [Display (Name ="Payment Condominium")]
        public int? CondominiumId { get; set; }


        //parte da criação da expense:

        [Required]
        [Display(Name = "Expense amount")]
        public decimal ExpenseAmount { get; set; }

        [Required]
        [Display(Name = "Expense detail")]
        public string ExpenseDetail { get; set; }


        [Required]
        [Display(Name = "Expense type")]
        public int? ExpenseTypeValue { get; set; }


        [BindNever]
        public List<SelectListItem>? ExpenseTypesList { get; set; }

    }
}
