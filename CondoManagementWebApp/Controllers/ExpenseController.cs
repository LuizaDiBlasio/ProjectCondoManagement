using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            _converterHelper = converterHelper;

        }


        


        //GET: ExpenseController
        public async Task<IActionResult> IndexExpenses()
        {

            try
            {
                var condominiumDto = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);

                var condoExpenses = await _apiCallService.GetByQueryAsync<IEnumerable<ExpenseDto>>("api/Expense/GetExpensesFromCondominium", this.User.Identity.Name);

                var model = new IndexExpensesViewModel()
                {
                    ExpensesDto = condoExpenses,
                    CondominiumName = condominiumDto.CondoName,
                };
                return View(model);

            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        // GET: ExpenseController/Create
        public async Task<IActionResult> CreateExpense()
        {
            try
            {
                var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

                if (expenseTypeList.Any())
                {
                    var model = new CreateEditExpenseViewModel()
                    {
                        ExpenseTypeDtoList = expenseTypeList
                    };

                    return View(model);
                }

                _flashMessage.Danger("Not possible to retrieve expenses type due to error");
                return View(new CreateEditExpenseViewModel());

            }
            catch
            {
                return View("Error");
            }
            
           
        }


        // POST: ExpenseController/Create
        [HttpPost]
        public async Task<ActionResult> RequestCreateExpense(CreateEditExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                _flashMessage.Danger("Unable to enter expense");
                return View(model);
            }

            try
            {
                var expenseDto = _converterHelper.ToExpenseDto(model);

                var condominiumDto = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);

                expenseDto.CondominiumId = condominiumDto.Id; 

                var expenseDtolWithExpenseTypeName = await SelectTypeName(expenseDto);

                var apiCall = await _apiCallService.PostAsync<ExpenseDto, Response<object>>("api/Expense/CreateExpense", expenseDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexExpenses));
                }

                model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                _flashMessage.Danger(apiCall.Message);
                return View("CreateExpense", model);
            }
            catch
            {
                model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                _flashMessage.Danger("Unable to enter expense");
                return View("CreateExpense", model);
            }
        }

        // GET: ExpenseController/Edit/5
        public async Task<ActionResult> EditExpense(int id)
        {
            var expenseTypeList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");

            if (expenseTypeList.Any() )
            {
                var expenseDto = await _apiCallService.GetAsync<ExpenseDto>($"api/Expense/GetExpense/{id}");
                if (expenseDto == null)
                {
                    return View("Error");
                }

                var model = new CreateEditExpenseViewModel()
                {
                    Id = expenseDto.Id,
                    Amount = expenseDto.Amount,
                    Detail = expenseDto.Detail,
                    ExpenseTypeDtoList = expenseTypeList,
                    ExpenseTypeDto =  expenseDto.ExpenseTypeDto
                };

                return View(model);
            }
            return View("Error");
        }

        // POST: ExpenseController/Edit/5
        [HttpPost]
        public async Task<ActionResult> RequestEditExpense(CreateEditExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                _flashMessage.Danger("Unable to modify expense");
                return View(model);
            }

            try
            {
                var expenseDto = await _apiCallService.GetAsync<ExpenseDto>($"api/Expense/GetExpense/{model.Id}");
                if (expenseDto == null)
                {
                    return View("Error");
                }

                var editedExpenseDto = _converterHelper.ToExpenseDto(model);

                //adicionar paymentId
                 editedExpenseDto.PaymentId = expenseDto.PaymentId; 

                var expenseDtolWithExpenseTypeName = await SelectTypeName(editedExpenseDto);

                var condominiumDto = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
                if (condominiumDto == null)
                {
                    model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                    _flashMessage.Danger("Unable to modify expense");
                    return View("EditExpense", model);
                }

                editedExpenseDto.CondominiumId = condominiumDto.Id;

                var apiCall = await _apiCallService.PostAsync<ExpenseDto, Response<object>>("api/Expense/EditExpense", editedExpenseDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexExpenses));
                }
                else
                {
                    _flashMessage.Danger(apiCall.Message);

                    model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                    return View("EditExpense", model);
                }

            }
            catch
            {
                model.ExpenseTypeDtoList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Expense/GetExpenseTypeList");
                _flashMessage.Danger("Unable to modify expense");
                return View("EditExpense", model);
            }
        }


        // POST: ExpenseController/Delete/5
        [HttpPost]
        public async Task<ActionResult> RequestDelete(int id)
        {
            try
            {
                var apiCall = await _apiCallService.PostAsync<int, Response<object>>("api/Expense/Delete", id);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexExpenses));
                }
                else
                {
                    _flashMessage.Danger(apiCall.Message);
                    //recarregar lista e nome do condominio
                    var model = await LoadIndexExpensesView();

                    return View("IndexExpenses", model);
                }     

                
            }
            catch 
            {
                return View("Error");
            }
        }

        //Metodos auxiliares

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

        public async Task<IndexExpensesViewModel> LoadIndexExpensesView()
        {
            var condominiumDto = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);

            var condoExpenses = await _apiCallService.GetByQueryAsync<IEnumerable<ExpenseDto>>("api/Expense/GetExpensesFromCondominium", this.User.Identity.Name);

            var model = new IndexExpensesViewModel()
            {
                ExpensesDto = condoExpenses,
                CondominiumName = condominiumDto.CondoName,
            };
            return model;
        }

        
    }
}
