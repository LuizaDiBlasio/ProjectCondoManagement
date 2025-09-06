using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances;
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
        private readonly DataContextCondos _dataContextCondos;

        public ExpenseController(DataContextFinances dataContextFinances, IConverterHelper converterHelper, IUserHelper userHelper, ICondominiumRepository condominiumRepository,
            IExpenseRepository expenseRepository, IPaymentRepository paymentsRepository, DataContextCondos dataContextCondos)
        {
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _condominiumRepository = condominiumRepository;
            _expensesRepository = expenseRepository;
            _paymentsRepository = paymentsRepository;
            _dataContextCondos = dataContextCondos;
        }


        // GET: Expense/GetExpensesFromCondominium
        [Microsoft.AspNetCore.Mvc.HttpGet("GetExpensesFromCondominiums")]
        public async Task<ActionResult<List<CondominiumWithExpensesDto>>> GetExpensesFromCondominiums()
        {
            var email = this.User.Identity?.Name;

            var user = await _userHelper.GetUserByEmailWithCompanyAsync(email);

            var condominiums =  _condominiumRepository.GetAll(_dataContextCondos).Where(c => c.ManagerUserId == user.Id).ToList();
            if (condominiums == null)
            {
                return new List<CondominiumWithExpensesDto>();
            }

            var condosWithExpensesDto = new List<CondominiumWithExpensesDto>();

            foreach (var condo in condominiums)
            {
                var condoExpenses = await _expensesRepository.GetExpensesFromCondominium(condo);

                var condoExpensesDto = condoExpenses?.Select(e => _converterHelper.ToExpenseDto(e, false)).ToList() ?? new List<ExpenseDto>();


                var condoWithExpensessDto = new CondominiumWithExpensesDto()
                {
                    CondominiumId = condo.Id,
                    CondoName = condo.CondoName,
                    ExpensesDto = condoExpensesDto
                };

                condosWithExpensesDto.Add(condoWithExpensessDto);
            }


            return Ok(condosWithExpensesDto);

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

                return Ok(new   Response<object>() { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response<object>() { IsSuccess = false, Message = "Unable to enter expense due to error" });
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
