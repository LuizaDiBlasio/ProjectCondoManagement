using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Models
{
    public class IndexExpensesViewModel
    {
        public IEnumerable<ExpenseDto>? ExpensesDto { get; set; } = new List<ExpenseDto>(); 

        public string? CondominiumName { get; set; } 
    }
}
