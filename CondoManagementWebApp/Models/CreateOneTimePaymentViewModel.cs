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


        public List<SelectListItem>? BeneficiaryTypeList { get; set; }

        public int SelectedBeneficiaryId { get; set; }

        public int? PayerFinancialAccountId { get; set; }

        //campo para beneficiário externo
        [Display(Name = "Recipient's Bank Account")]
        public string? ExternalRecipientBankAccount { get; set; }


        //fazer validação no controller
        [Display(Name = "Recipient's Account")]
        public int? BeneficiaryAccountId { get; set; }

        public SelectList? CondosToSelect { get; set; }

        [Required]
        [Display (Name ="Payment Condominium")]
        public int? CondominiumId { get; set; }

        public string? Recipient { get; set; }

        //parte da criação da expense:

        [Required]
        [Display(Name = "Expense amount")]
        public decimal ExpenseAmount { get; set; }

        [Display(Name = "Select Member")]
        public SelectList? CondoMembers { get; set; } 

        [Required]
        [Display(Name = "Expense detail")]
        public string ExpenseDetail { get; set; }


        [Required]
        [Display(Name = "Expense type")]
        public int? ExpenseTypeValue { get; set; }


        [BindNever]
        public List<SelectListItem>? ExpenseTypesList { get; set; }

        [Required]
        public string SelectedPayerType { get; set; } 

        public int? SelectedCondoMemberId { get; set; } 

    }
}
