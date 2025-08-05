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
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var condoMember = await _condoMemberHelper.GetCondoMemberAsync(id.Value);

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
            catch
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(condoMemberDto);
            }

        }

        // GET: CondoMemberController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return  NotFound();
            }

            var condoMember = await _condoMemberHelper.GetCondoMemberAsync(id.Value);
            if (condoMember == null)
            {
                return NotFound();
            }


            return View(condoMember);
        }

        // POST: CondoMemberController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CondoMemberDto condoMemberDto)
        {

            if (id != condoMemberDto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(condoMemberDto);
            }

            try
            {

               var success = await _condoMemberHelper.EditCondoMemberAsync(condoMemberDto);
               if (!success)
               {
                   ModelState.AddModelError("", "Failed to edit condo member. Please try again.");
                   return View(condoMemberDto);
               }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(condoMemberDto);
            }
        }

        // GET: CondoMemberController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condoMember = await _condoMemberHelper.GetCondoMemberAsync(id.Value);
            if (condoMember == null)
            {
                return NotFound();
            }


            return View(condoMember);
        }

        // POST: CondoMemberController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {                            

                var condoMember = await _condoMemberHelper.GetCondoMemberAsync(id.Value);
                if (condoMember == null)
                {
                    return NotFound();
                }

                var success = await _condoMemberHelper.DeleteCondoMemberAsync(id.Value);
                if (!success)
                {
                    ModelState.AddModelError("", "Failed to delete condo member. Please try again.");
                    return View(condoMember);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(id);
            }
        }
    }
}
