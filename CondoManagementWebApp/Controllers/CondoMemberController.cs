using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CondoManagementWebApp.Controllers
{
    public class CondoMemberController : Controller
    {
        private readonly ICondoMemberHelper _condoMemberHelper;

        public CondoMemberController(ICondoMemberHelper condoMemberHelper)
        {
            _condoMemberHelper = condoMemberHelper;
        }


        // GET: CondoMemberController
        public async Task<ActionResult> Index()
        {
            var condoMembers = await  _condoMemberHelper.GetCondoMembersAsync();

            return View(condoMembers);
        } 

        // GET: CondoMemberController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var condoMember = await _condoMemberHelper.GetCondoMemberAsync(id);

            return View(condoMember);
        }

        // GET: CondoMemberController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CondoMemberController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CondoMemberDto condoMemberDto)
        {
            if (!ModelState.IsValid)
            {
                return View(condoMemberDto);
            }

            try
            {
                var success = await _condoMemberHelper.CreateCondoMemberAsync(condoMemberDto);

                if (!success)
                {
                    ModelState.AddModelError("", "Failed to create condo member. Please try again.");
                    return View(condoMemberDto);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(condoMemberDto);
            }

        }

        // GET: CondoMemberController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CondoMemberController/Edit/5
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

        // GET: CondoMemberController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CondoMemberController/Delete/5
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
