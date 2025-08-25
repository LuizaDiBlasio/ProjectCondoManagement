using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly DataContextFinances _dataContextFinances;
        private readonly IConverterHelper _converterHelper;

        public PaymentController(IPaymentRepository paymentRepository, DataContextFinances dataContextFinances, IConverterHelper converterHelper)
        {
            _paymentRepository = paymentRepository;
            _dataContextFinances = dataContextFinances;
            _converterHelper = converterHelper;
        }


        // GET: PaymensController
        [Microsoft.AspNetCore.Mvc.HttpGet("GetPaymentsFromCondoMember")]
        public List<PaymentDto> GetPaymentsFromCondoMember([FromBody] string userEmail)
        {
            // get user payments
            var allPayments = _paymentRepository.GetAll(_dataContextFinances);

            var userPayments = allPayments.Where(p => p.UserEmail == userEmail).ToList();

            //converter
            var userPaymentsDto = userPayments.Select(p => _converterHelper.ToPaymentDto(p, false)).ToList() ?? new List<PaymentDto>();

            return userPaymentsDto;
        }


        // GET: PaymensController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PaymensController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymensController/Create
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

        // GET: PaymensController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaymensController/Edit/5
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

        // GET: PaymensController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PaymensController/Delete/5
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
