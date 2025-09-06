using ClassLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.Enums;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class ExpenseRepository : GenericRepository<Expense, DataContextFinances>, IExpenseRepository
    {
        private readonly DataContextFinances _dataContextFinances;

        public ExpenseRepository(DataContextFinances dataContextFinances)
        {
            _dataContextFinances = dataContextFinances;
        }
        public async Task<List<Expense>> GetExpensesFromCondominium(Condominium condo)
        {
            
            return await _dataContextFinances.Expenses
                       .Where(e => e.CondominiumId == condo.Id)
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

        public async Task<List<SelectListItem>> GetExpensesList(int condoId)
        {
           var condoExpenses =  await _dataContextFinances.Expenses.Where(e => e.CondominiumId == condoId).ToListAsync();

            return condoExpenses.Select(ce => new SelectListItem
            {
               Value = ce.Id.ToString(),
               Text = ce.ToString() 
            }).ToList();
        }
    }
}
