using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CondoManagementWebApp.Models
{
    public class CreateEditExpenseViewModel
    {
        
        public int Id { get; set; }


        [Required]
        public decimal Amount { get; set; }


        [Required]
        [StringLength(50)]
        public string Detail { get; set; }


        [Required]
        [Display(Name = "Type")]
        public EnumDto ExpenseTypeDto { get; set; }


        public List<SelectListItem>? ExpenseTypeDtoList { get; set; }
    }
}
