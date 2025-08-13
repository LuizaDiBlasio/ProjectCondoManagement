using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
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

            var condominiums = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");

            return View();
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

            var result = await _apiCallService.PostAsync<CondominiumDto, Response>("api/Condominiums", condominiumDto);


            if (result.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }           
            

            ModelState.AddModelError("", result.Message);
            return View(condominiumDto);
            
        }

        // GET: ConduminiumController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condominium = _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");
            if (condominium == null)
            {
                return NotFound();
            }

            return View(condominium);
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


            var result = await _apiCallService.PostAsync<CondominiumDto, Response>($"api/Condominiums/Edit/{id}", condominiumDto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(condominiumDto);
            }

            return RedirectToAction(nameof(Index));
            
        }

        // GET: ConduminiumController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }   

            var condominium = _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{id.Value}");

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
    }
}
