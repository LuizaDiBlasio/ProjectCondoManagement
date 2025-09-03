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


      

       

        
    }
}
