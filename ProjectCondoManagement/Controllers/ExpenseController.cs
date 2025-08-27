using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Text.Json;


namespace ProjectCondoManagement.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExpenseController : Microsoft.AspNetCore.Mvc.Controller
    {

        private readonly DataContextFinances _dataContextFinances;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ICondominiumRepository _condominiumRepository;
        private readonly IExpenseRepository _expensesRepository;
        private readonly IPaymentRepository _paymentsRepository;

        public ExpenseController(DataContextFinances dataContextFinances, IConverterHelper converterHelper, IUserHelper userHelper, ICondominiumRepository condominiumRepository,
            IExpenseRepository expenseRepository, IPaymentRepository paymentsRepository)
        {
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
            _expensesRepository = expenseRepository;
            _paymentsRepository = paymentsRepository;
        }


        // GET: Expense/GetExpensesFromCondominium
        [Microsoft.AspNetCore.Mvc.HttpPost("GetExpensesFromCondominium")]
        public async Task<ActionResult<List<ExpenseDto>>> GetExpensesFromCondominium([FromBody] JsonElement condoManagerEmail)
        {
            var email = condoManagerEmail.GetString();

            var user = await _userHelper.GetUserByEmailAsync(email);

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
        [Microsoft.AspNetCore.Mvc.HttpGet("GetExpense/{id}")]
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

                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to enter expense due to error" });
            }
        }



        // POST: Expense/Edit/5
        [Microsoft.AspNetCore.Mvc.HttpPost("EditExpense")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> EditExpense([FromBody] ExpenseDto expenseDto)
        {
            if (expenseDto == null)
            {
                return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to modify, expensa not found" });
            }

            try
            {
                var expense = _converterHelper.ToExpense(expenseDto, false);

                await _expensesRepository.UpdateAsync(expense, _dataContextFinances);

                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to modify expense due to error" });
            }
        }

        // GET: Expense/Delete/5
        [Microsoft.AspNetCore.Mvc.HttpPost("Delete")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete([FromBody] int id)
        {
            try
            {
                var expense = await _expensesRepository.GetByIdAsync(id, _dataContextFinances);

                if (expense == null)
                {
                    return NotFound(new Response<object> { IsSuccess = false, Message = "Unable to delete, expensa not found" });
                }

                //despesa não pode fazer parte de nenhum pagamento 

                var allPayments = _paymentsRepository.GetAll(_dataContextFinances);
                if (allPayments != null)
                {
                    if (allPayments.Any(p => p.Expenses.Any(e => e.Id == id))) //caso haja pagamentos, ver se existe alguma despesa com esse id
                    {
                        return Conflict(new Response<object> { IsSuccess = false, Message = "Unable to delete, expense included in issued payments" });
                    }

                }


                await _expensesRepository.DeleteAsync(expense, _dataContextFinances);

                return Ok(new Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object> { IsSuccess = false, Message = "Unable to delete due to server error" });
            }
        }




        //Medotos auxiliares

        [Microsoft.AspNetCore.Mvc.HttpGet("GetExpenseTypeList")]
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetExpenseTypeList()
        {
            return _expensesRepository.GetExpenseTypeList();
        }


        [Microsoft.AspNetCore.Mvc.HttpGet("GetExpensesList/{id}")]
        public async Task<List<SelectListItem>> GetExpensesList(int id)
        {
            return await _expensesRepository.GetExpensesList(id);
        }

    }
}
