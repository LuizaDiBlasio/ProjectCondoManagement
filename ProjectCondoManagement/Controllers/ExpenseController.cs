using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Web.Mvc;

namespace ProjectCondoManagement.Controllers
{
    public class ExpenseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly DataContextFinances _dataContextFinances;
        private readonly IConverterHelper _converterHelper;

        public ExpenseController(IPaymentRepository paymentRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper)
        {
            _paymentRepository = paymentRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;    
        }


        // GET: Expense
        [Microsoft.AspNetCore.Mvc.HttpGet("GetExpensesFromUser")]
        public List<ExpenseDto> GetExpensesFromUser([FromBody] string userEmail)
        {
            // get user payments
            var allPayments = _paymentRepository.GetAll(_dataContextFinances);

            var userPayments = allPayments.Where(p => p.UserEmail == userEmail).ToList();

            //converter
            var userPaymentsDto = userPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

            //buscar na lista de despesas de cada pagamento e adicionar as despesas à nova lista 
            var userExpensesDto = userPaymentsDto.SelectMany(payment => payment.ExpensesDto).ToList();

            return userExpensesDto;    
        }   

        


        // GET: Expense/Details/5
        public Microsoft.AspNetCore.Mvc.ActionResult Details(int id)
        {
            return View();
        }

        // GET: Expense/Create
        public Microsoft.AspNetCore.Mvc.ActionResult Create()
        {
            return View();
        }

        // POST: Expense/Create
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.ActionResult Create(IFormCollection collection)
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

        // GET: Expense/Edit/5
        public Microsoft.AspNetCore.Mvc.ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Expense/Edit/5
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.ActionResult Edit(int id, IFormCollection collection)
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

        // GET: Expense/Delete/5
        public Microsoft.AspNetCore.Mvc.ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Expense/Delete/5
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.ActionResult Delete(int id, IFormCollection collection)
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
