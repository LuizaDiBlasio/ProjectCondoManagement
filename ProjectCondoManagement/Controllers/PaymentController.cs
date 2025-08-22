using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectCondoManagement.Controllers
{
    public class PaymentController : Controller
    {
        // GET: PaymensController
        public ActionResult Index()
        {
            return View();
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
