using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CondoManagementWebApp.Controllers
{
    public class CondoMemberController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IConverterHelper _converterHelper;

        public CondoMemberController(IApiCallService apiCallService, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
        }


        // GET: CondoMemberController
        public async Task<ActionResult> Index()
        {
            var condoMembers = await _apiCallService.GetAsync<IEnumerable<CondoMemberDto>>("api/CondoMembers");

            return View(condoMembers);
        } 

        // GET: CondoMemberController/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");

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

            if(condoMemberDto.BirthDate > DateTime.Today)
            {
                ModelState.AddModelError("BirthDate", "Birth date cannot be in the future.");
            }

                var registerUserDto = _converterHelper.ToRegisterDto(condoMemberDto);

                registerUserDto.SelectedRole = "CondoMember"; 

                var result2 = await _apiCallService.PostAsync<RegisterUserDto, Response>("api/Account/AssociateUser", registerUserDto);
                if (!result2.IsSuccess)
                {
                    ModelState.AddModelError("", $"{result2.Message}");
                    return View(condoMemberDto);
                }
 

                var result = await _apiCallService.PostAsync<CondoMemberDto, Response>("api/CondoMembers", condoMemberDto);



                if (!result.IsSuccess)
                {
                    ModelState.AddModelError("", $"{result.Message}");
                    return View(condoMemberDto);
                }

                

                return RedirectToAction(nameof(Index));
          

        }

        // GET: CondoMemberController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return  NotFound();
            }

            var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
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

               var result = await _apiCallService.PostAsync<CondoMemberDto, Response>($"api/CondoMembers/Edit/{id}", condoMemberDto);
               if (!result.IsSuccess)
               {
                   ModelState.AddModelError("", $"{result.Message}");
                   return View(condoMemberDto);
               }

                return RedirectToAction(nameof(Index));
           
        }

        // GET: CondoMemberController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}"); ;
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

                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
                if (condoMember == null)
                {
                    return NotFound();
                }

                var result = await _apiCallService.DeleteAsync($"api/CondoMembers/{id.Value}");
                if (!result.IsSuccessStatusCode)
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
