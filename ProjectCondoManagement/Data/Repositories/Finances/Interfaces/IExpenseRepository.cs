using ClassLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense, DataContextFinances>
    {
        Task<List<Expense>> GetExpensesFromCondominium(Condominium condo);

        List<SelectListItem> GetExpenseTypeList();

        Task<List<SelectListItem>> GetExpensesList(int condoId);
    }
}
