using ClassLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.Enums;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class ExpenseRepository : GenericRepository<Expense, DataContextFinances>, IExpenseRepository
    {
        private readonly DataContextFinances _dataContextFinances;  
        public async Task<List<Expense>> GetExpensesFromCondominium(Condominium condoManagerCondo)
        {
            return await _dataContextFinances.Expenses
                        .Where(e => e.Condominium.Id == condoManagerCondo.Id)
                        .ToListAsync();
        }

        public List<SelectListItem> GetExpenseTypeList()
        {
            return Enum.GetValues(typeof(ExpenseType)) //indicar o tipo do enum
                    .Cast<ExpenseType>() // converter ints do enum para lista IEnumerable<MessageStatus>
                     .Select(status => new SelectListItem //converter para lista SelectListItem 
                     {
                         Value = ((int)status).ToString(),
                         Text = status.ToString()
                     }).ToList();

        }
    }
}
