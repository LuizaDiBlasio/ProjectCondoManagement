using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class CondoMemberController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public CondoMemberController(IApiCallService apiCallService, IConverterHelper converterHelper, IFlashMessage flashMessage)
        {
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }


        // GET: CondoMemberController
        public async Task<ActionResult> Index()
        {
            try
            {
                var condoMembers = await _apiCallService.GetAsync<IEnumerable<CondoMemberDto>>("api/CondoMembers")?? new List<CondoMemberDto>();
                return View(condoMembers);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching condo members.");
                return View(new List<CondoMemberDto>());
            }


        }

        // GET: CondoMemberController/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");

                return View(condoMember);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching the condo member.");
                return View(new CondoMemberDto());
            }
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

            if (condoMemberDto.BirthDate > DateTime.Today)
            {
                ModelState.AddModelError("BirthDate", "Birth date cannot be in the future.");
                return View(condoMemberDto);
            }

            try
            {
                var result = await _apiCallService.PostAsync<CondoMemberDto, Response<object>>("api/CondoMembers", condoMemberDto);
                if (result == null)
                {
                    _flashMessage.Danger($"An error occurred while creating the condo member.");
                    return View(condoMemberDto);
                }

                if (result.IsSuccess)
                {
                    var registerUserDto = _converterHelper.ToRegisterDto(condoMemberDto);

                    registerUserDto.SelectedRole = "CondoMember";

                    var result2 = await _apiCallService.PostAsync<RegisterUserDto, Response<object>>("api/Account/AssociateUser", registerUserDto);
                    if (!result2.IsSuccess)
                    {
                        var createdMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/ByEmail/{condoMemberDto.Email}");
                        if (createdMember == null)
                        {
                            return NotFound("Condo member not found after creation.");
                        }

                        await _apiCallService.DeleteAsync($"api/CondoMembers/{createdMember.Id}");
                        ModelState.AddModelError("", $"{result2.Message}");
                        return View(condoMemberDto);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {

                _flashMessage.Danger($"An error occurred while creating the condo member.");
                return View(condoMemberDto);
            }


            _flashMessage.Danger($"An error occurred while creating the condo member.");
            return View(condoMemberDto);

        }



        // GET: CondoMemberController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
                return View(condoMember);
            }
            catch (Exception)
            {

               _flashMessage.Danger($"An error occurred while fetching the condo member for editing.");
               return View(new CondoMemberDto());
            }

  
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

            var result = await _apiCallService.PostAsync<CondoMemberDto, Response<object>>($"api/CondoMembers/Edit/{id}", condoMemberDto);
            if (result == null)
            {
                _flashMessage.Danger($"An error occurred while updating the condo member.");
                return View(condoMemberDto);
            }
            if (!result.IsSuccess)
            {
                _flashMessage.Danger($"An error occurred while updating the condo member.");
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

            try
            {
                var condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id.Value}");
                if (condoMember == null)
                {
                    return NotFound();
                }

                return View(id);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"An error occurred while fetching the condo member for deletion.");
                return RedirectToAction(nameof(Index));
            }            
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
                    _flashMessage.Danger("Failed to delete condo member. Please try again.");
                    return View(condoMember);
                }


                return RedirectToAction(nameof(Index));
            }
            catch(Exception)
            {
                _flashMessage.Danger("An unexpected error occurred while deleting the condo member.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}