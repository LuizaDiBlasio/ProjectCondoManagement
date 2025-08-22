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
        public string Detail { get; set; }


        [Required]  
        public CondominiumDto CondominiumDto { get; set; }

        [Required]
        public EnumDto ExpenseTypeDto { get; set; }

        public List<SelectListItem> ExpenseTypeDtoList { get; set; }
    }
}
