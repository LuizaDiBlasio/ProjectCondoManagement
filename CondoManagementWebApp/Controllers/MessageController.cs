using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CondoManagementWebApp.Controllers
{
    public class MessageController : Controller
    {
        // GET: MessageController
        public ActionResult Inbox()
        {
            return View();
        }

        public ActionResult SentMessages()
        {
            return View();
        }

        // GET: MessageController/Details/5
        public ActionResult MessageDetails(int id)
        {
            return View();
        }

        // POST: MessageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeMessageDetails(int id, IFormCollection collection)
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

        // GET: MessageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MessageController/Delete/5
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
