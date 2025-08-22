using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class ExpenseController : Controller

    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;

        public ExpenseController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
             _apiCallService = apiCallService;
            _flashMessage = flashMessage;

        }


        //GET: ExpenseController
        public async Task<ActionResult<List<ExpenseDto>>> IndexExpenses()
        {
            try
            {      
                var condoExpenses = await _apiCallService.GetByQueryAsync<IEnumerable<ExpenseDto>>("api/Expense/GetExpensesFromCondominium", this.User.Identity.Name);

                if (condoExpenses == null)
                {
                    var noExpenses = new List<ExpenseDto>();    
                    return View(noExpenses);  
                }

                return View(condoExpenses);
                
            }
            catch (Exception)
            {
                return View("Error");
            }
        }



        // GET: ExpenseController/Create
        public async Task<IActionResult> CreateExpense()
        {
            var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

            var condominiumDto = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominium/GetCondoManagerCondominiumDto", this.User.Identity.Name);

            if (expenseTypeList.Any() && condominiumDto != null)
            {
                var model = new CreateEditExpenseViewModel()
                {
                    CondominiumDto = condominiumDto,
                    ExpenseTypeDtoList = expenseTypeList    
                };

                return View(model); 
            }
            return View("Error");
        }


        // POST: ExpenseController/Create
        [HttpPost]
        public async Task<ActionResult> RequestCreateExpense(CreateEditExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to enter expense");
                return View(model); 
            }

            try
            {
                var expenseDto = _converterHelper.ToExpenseDto(model);

                var expenseDtolWithExpenseTypeName = await SelectTypeName(expenseDto);

                var apiCall = await _apiCallService.PostAsync<ExpenseDto, Response>("api/Expense/CreateExpense", expenseDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexExpenses));
                }

                _flashMessage.Danger(apiCall.Message);
                return View("CreateExpense", model);
            }
            catch
            {
                _flashMessage.Danger("Unable to enter expense");
                return View("CreateExpense", model);
            }
        }

        // GET: ExpenseController/Edit/5
        public async Task<ActionResult> EditExpense(int id)
        {
            var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

            var condominiumDto = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominium/GetCondoManagerCondominiumDto", this.User.Identity.Name);

            if (expenseTypeList.Any() && condominiumDto != null)
            {
                var expenseDto = await _apiCallService.GetAsync<ExpenseDto>($"api/Expense/GetExepense/{id}");
                if(expenseDto == null)
                {
                    return View("Error");
                }

                var model = new CreateEditExpenseViewModel()
                {
                    Id = expenseDto.Id,
                    Amount = expenseDto.Amount,
                    Detail = expenseDto.Detail, 
                    CondominiumDto = condominiumDto,
                    ExpenseTypeDtoList = expenseTypeList
                };

                return View(model);
            }
            return View("Error");
        }

        // POST: ExpenseController/Edit/5
        [HttpPost]
        public async Task<ActionResult> RequestEdit(CreateEditExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to modify expense");
                return View(model);
            }

            try
            {
                var expenseDto = _converterHelper.ToExpenseDto(model);

                var expenseDtolWithExpenseTypeName = await SelectTypeName(expenseDto);

                var apiCall = await _apiCallService.PostAsync<ExpenseDto, Response>("api/Expense/EditExpense", expenseDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexExpenses));
                }

                _flashMessage.Danger(apiCall.Message);
                return View("CreateExpense", model);
            }
            catch
            {
                _flashMessage.Danger("Unable to modify expense");
                return View("CreateExpense", model);
            }
        }


        // POST: ExpenseController/Delete/5
        [HttpPost]
        public async Task<ActionResult> RequestDelete(int id)
        {
            try
            {
                var apiCall = await _apiCallService.PostAsync<int, Response>("api/Expense/Delete", id);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexExpenses));
                }

                _flashMessage.Danger(apiCall.Message);
                return View("IndexExpenses");
            }
            catch
            {
                _flashMessage.Danger("Unable to delete");
                return View("IndexExpenses");
            }
        }

        //Metodo auxiliar
        public async Task<ExpenseDto> SelectTypeName(ExpenseDto expenseDto)
        {
            //Fazer seleção do Name do status (a select list só preenche o value)
            var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

            var selectedExpenseType = expenseTypeList.FirstOrDefault(e => e.Value == expenseDto.ExpenseTypeDto.Value.ToString());

            if (selectedExpenseType != null)
            {
                // Preencher Name do EnumDto antes de enviar para a API
                expenseDto.ExpenseTypeDto.Name = selectedExpenseType.Text;
            }

            return expenseDto;
        }
    }
}
