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
        public async Task<ActionResult<List<CondominiumWithExpensesDto>>> IndexExpenses()
        {

            try
            {        
                var condosExpenses = await _apiCallService.GetAsync<List<CondominiumWithExpensesDto>>("api/Expense/GetExpensesFromCondominiums");

                
                return View(condosExpenses);

            }
            catch (Exception)
            {
                return View("Error");
            }
        }


      

       

        
    }
}
