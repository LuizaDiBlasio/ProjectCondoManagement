using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CondoManagementWebApp.Controllers
{
    public class ExpenseController : Controller

    {
        private readonly IApiCallService _apiCallService;

        public ExpenseController(IApiCallService apiCallService)
        {
             _apiCallService = apiCallService;

        }


        // GET: ExpenseController
        //public async Task<ActionResult<List<ExpenseDto>>> IndexExpenses()
        //{
        //    try
        //    {
        //        if (User.IsInRole("CondoMember"))
        //        {
        //            //Buscar expenses do condomember - conta individual no sistema
        //            var condoMemberExpenses = await _apiCallService.GetByQueryAsync<IEnumerable<ExpenseDto>>("api/Expense/GetExpensesFromUser", this.User.Identity.Name);

        //            return View(condoMemberExpenses);
        //        }

        //        //if (User.IsInRole("CompanyAdmin"))
        //        //{
        //        //    var companyExpenses = 
        //        //}
                
        //    }
        //    catch (Exception)
        //    {
        //        return View("Error");
        //    }
        //}


        // GET: ExpenseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExpenseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExpenseController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ExpenseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExpenseController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ExpenseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
