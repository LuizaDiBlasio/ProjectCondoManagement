using ClassLibrary.DtoModels;
using CondoManagementWebApp.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateRecurringPaymentViewModel
    {


        [DateGreaterThanToday(ErrorMessage = "Issue Date must be a future date.")]
        [Required(ErrorMessage = "The due date is required.")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }


        [Required]
        [Display(Name = "Omah wallet number")]
        public int PayerFinancialAccountId { get; set; }

        public int CondominiumId { get; set; }


        [Required]
        [Display(Name="Select payment expenses")]
        public List<int> SelectedExpensesIds { get; set; }


        [BindNever]
        public List<SelectListItem> ExpensesToSelect { get; set; } = new List<SelectListItem>();

    }
}
