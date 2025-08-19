using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class CondominiumController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public CondominiumController(IApiCallService apiCallService, IConverterHelper converterHelper, IFlashMessage flashMessage)
        {
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }

        // GET: ConduminiumController
        public async Task<IActionResult> Index()
        {
            IEnumerable<CondominiumDto> condominiums = new List<CondominiumDto>();

            try
            {
                condominiums = await _apiCallService.GetAsync<IEnumerable<CondominiumDto>>("api/Condominiums");

                return View(condominiums);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"{ex.Message}");               
            }


            

            return View(condominiums);
        }

        // GET: ConduminiumController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var condominium = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");
                if (condominium == null)
                {
                    return NotFound();
                }

                return View(condominium);
            }
            catch (Exception)
            {
                _flashMessage.Danger($"Error feetching condominium");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ConduminiumController/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: ConduminiumController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CondominiumDto condominiumDto)
        {

            if (!ModelState.IsValid)
            {
                return View(condominiumDto);
            }

            try
            {
                var result = await _apiCallService.PostAsync<CondominiumDto, Response>("api/Condominiums", condominiumDto);

                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                _flashMessage.Danger(result.Message);
                return View(condominiumDto);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Erro posting condominium{ex.InnerException}");
                return View(condominiumDto);
            }                              
            
        }


        // GET: ConduminiumController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var condominium = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");
                if (condominium == null)
                {
                    return NotFound();
                }

                return View(condominium);
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error retrieving condominium for edit.");
                return RedirectToAction(nameof(Index));
            }



        }

        // POST: ConduminiumController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Edit(int id, CondominiumDto condominiumDto)
        {
            if (id != condominiumDto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(condominiumDto);
            }


            try
            {
                var result = await _apiCallService.PostAsync<CondominiumDto, Response>($"api/Condominiums/Edit/{id}", condominiumDto);
                if (!result.IsSuccess)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(condominiumDto);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error updating condominium");                
            }

            return View(condominiumDto);
            
        }

        // GET: ConduminiumController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var condominium = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");
                return View(condominium);
            }
            catch (Exception)
            {

                _flashMessage.Danger("Error fetching condominium to delete");
            }

            return View();
        }

        // POST: ConduminiumController/Delete/5
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

                var condominium = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");
                if (condominium == null)
                {
                    return NotFound();
                }

                var result = await _apiCallService.DeleteAsync($"api/Condominiums/{id.Value}");
                if (!result.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to delete condo member. Please try again.");
                    return View(condominium);
                }


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(id);
            }
        }


        public async Task<IActionResult> AssignManager(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var condominium = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");
                if (condominium == null)
                {
                    return NotFound();
                }

                var managers = await  _apiCallService.GetAsync<IEnumerable<UserDto>>("api/Account/GetManagers");
                if (managers == null)
                {
                    managers = new List<UserDto>(); 
                }


                var model = new AssignManagerViewModel
                {
                    Id = condominium.Id,
                    CompanyId = condominium.CompanyId,
                    Company = condominium.Company,
                    Address = condominium.Address,
                    CondoName = condominium.CondoName,
                    ManagerUserId = condominium.ManagerUserId,
                    Managers = managers
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("Error retrieving condominium.");
                return RedirectToAction(nameof(Index));
            }


        }



        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignManager(int id, AssignManagerViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var condo = new CondominiumDto
            {
                Id = model.Id,
                CompanyId = model.CompanyId,
                Address = model.Address,
                CondoName = model.CondoName,
                ManagerUserId = model.ManagerUserId
            };

            try
            {
                var result = await _apiCallService.PostAsync<CondominiumDto, Response>($"api/Condominiums/Edit/{id}", condo);
                if (!result.IsSuccess)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error assigning manager");
            }

            return View(model);

        }



    }
}
