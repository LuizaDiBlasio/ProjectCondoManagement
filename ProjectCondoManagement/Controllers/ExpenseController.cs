using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Controllers
{
    public class ExpenseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly DataContextFinances _dataContextFinances;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IExpenseRepository _expensesRepository;

        public ExpenseController(IPaymentRepository paymentRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper, IUserHelper userHelper, ICondominiumRepository condominiumRepository,
            IExpenseRepository expenseRepository)
        {
            _paymentRepository = paymentRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
            _expensesRepository = expenseRepository;
        }


        // GET: Expense/GetExpensesFromCondominium
        [HttpGet("GetExpensesFromCondominium")]
        public async Task<ActionResult<List<ExpenseDto>>> GetExpensesFromCondominium([FromBody] string condoManagerEmail)
        {
            var user = await _userHelper.GetUserByEmailAsync(condoManagerEmail);

            if (user == null)
            {
                return NotFound();
            }

            var condoManagerCondo = await _condominiumRepository.GetCondoManagerCondominium(user.Id);

            if (condoManagerCondo == null)
            {
                return NotFound();
            }

            var condominiumExpenses = await _expensesRepository.GetExpensesFromCondominium(condoManagerCondo);

            if (condominiumExpenses == null)
            {
                return NotFound();
            }

            var condominiumExpensesDto = condominiumExpenses?.Select(e => _converterHelper.ToExpenseDto(e, false)) ?? new List<ExpenseDto>();

            return Ok(condominiumExpensesDto);

        }

        //GET : Expense/GetExpense/5
        [HttpGet("GetExepense/{id}")]
        public async Task<ActionResult<ExpenseDto>> GetExpense(int id)
        {
            var expense = await _expensesRepository.GetByIdAsync(id, _dataContextFinances);

            if (expense == null)
            {
                return NotFound();  
            }

            var expenseDto = _converterHelper.ToExpenseDto(expense, false);

            return Ok(expenseDto);  

        }



        // POST: Expense/CreateExpense
        [Microsoft.AspNetCore.Mvc.HttpPost("CreateExpense")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateExpense([FromBody] ExpenseDto expenseDto)
        {
            if (expenseDto == null)
            {
                return BadRequest();    
            }

            try
            {
                var expense = _converterHelper.ToExpense(expenseDto, true);

                await _expensesRepository.CreateAsync(expense, _dataContextFinances);

                return Ok(new Response() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response () { IsSuccess = false, Message = "Unable to enter expense due to error" });   
            }
        }

      

        // POST: Expense/Edit/5
        [Microsoft.AspNetCore.Mvc.HttpPost("EditExpense")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit([FromBody] ExpenseDto expenseDto)
        {
            if (expenseDto == null)
            {
                return NotFound(new Response { IsSuccess = false, Message = "Unable to modify, expensa not found" });
            }

            try
            {
                var expense = _converterHelper.ToExpense(expenseDto, true);

                await _expensesRepository.UpdateAsync(expense, _dataContextFinances);

                return Ok(new Response() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable to modify expense due to error" });
            }
        }

        // GET: Expense/Delete/5
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete([FromBody] int id)
        {
            try
            {
                var expense = await _expensesRepository.GetByIdAsync(id, _dataContextFinances);

                if(expense == null)
                {
                    return NotFound(new Response { IsSuccess = false, Message = "Unable to delete, expensa not found"});
                }

                await _expensesRepository.DeleteAsync(expense, _dataContextFinances);

                return Ok(new Response() {IsSuccess = true});
            }
            catch
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Unable to delete due to server error" });
            }
        }

        

        //Medoto auxiliar

        public List<SelectListItem> GetExpenseTypeList()
        {
            return _expensesRepository.GetExpenseTypeList();
        }
    }
}
