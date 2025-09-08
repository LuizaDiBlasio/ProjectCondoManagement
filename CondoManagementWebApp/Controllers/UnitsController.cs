using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Syncfusion.EJ2.Charts;
using System.Web.Helpers;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class UnitsController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public UnitsController(IApiCallService apiCallService, IConverterHelper converterHelper, IFlashMessage flashMessage)
        {
            _apiCallService = apiCallService;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }


        // GET: UnitsController
        public async Task<IActionResult> Index(int? id, string? condoName)
        {
            IEnumerable<UnitDto> units = new List<UnitDto>();

            try
            {
                if(id == null)
                {
                    units = await _apiCallService.GetAsync<IEnumerable<UnitDto>>("api/Units");
                }
                else
                {
                    if(condoName != null)
                    {
                        ViewBag.CondoName = condoName;
                    }

                   
                    ViewBag.CondominiumId = id;

                    units = await _apiCallService.GetAsync<IEnumerable<UnitDto>>($"api/Units/condo/{id.Value}");

                }

                    return View(units);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Error fetching units");
                return View(units);
            }

            
        }

        // GET: UnitsController
        public async Task<IActionResult> MemberUnits(int? id)
        {
            if (!this.User.IsInRole("CondoMember"))
            {
                if (id == null)
                {
                    return NotFound();
                }
            }
           

         
            IEnumerable<UnitDto> units = new List<UnitDto>();

            try
            {
                var condoMember = new CondoMemberDto();

                if (this.User.IsInRole("CondoMember"))
                {
                    var email = this.User.Identity?.Name;

                   condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/ByEmail/{email}");
                }
                else
                {
                    condoMember = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/{id}");
                }

                if (condoMember == null)
                {
                    return NotFound();
                }

                if (condoMember.Units != null)
                {
                    units = condoMember.Units;
                }

                ViewBag.MemberName = condoMember.FullName;
                ViewBag.MemberId = condoMember.Id;

                return View(units);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Error loading units.");

                return View(units);
            }

           
        }



        // GET: UnitsController/Details/5
        public async Task<IActionResult> Details(int? id, int? condoId, string? condoName, int? memberId, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var unit = await _apiCallService.GetAsync<UnitDto>($"api/Units/{id.Value}");
                if (unit == null)
                {
                    return NotFound();
                }

                if (condoId != null && condoName != null)
                {
                    ViewBag.CondominiumId = condoId;
                    ViewBag.CondoName = condoName;

                }

                if(returnUrl != null)
                {
                    ViewBag.ReturnUrl = returnUrl;
                }

                if (memberId != null)
                {
                    ViewBag.MemberId = memberId;
                }

                return View(unit);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Error fetching unit");

                if(returnUrl != null & condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(UnitsFromCondo), new { condoId = condoId, condoName = condoName });
                }

                if (condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                }

                if (memberId != null)
                {
                    return RedirectToAction(nameof(MemberUnits), new { id = memberId });
                }

                return RedirectToAction(nameof(Index));
            }

            
        }

        // GET: UnitsController/Create
        public async  Task<IActionResult> Create(int? memberId, int? condoId, string? condoName)
        {

            var model = new UnitDtoViewModel();

            if (condoId != null && condoName != null)
            {
                model.CondoId = condoId;
                model.CondoName = condoName;
            }

            else
            {
                var condos = await _apiCallService.GetAsync<IEnumerable<CondominiumDto>>("api/Condominiums");

                model.Condos = condos.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), 
                    Text = c.CondoName              
                }).ToList();
            }

            if (memberId != null)
            {
                model.MemberId = memberId.Value;
            }


                return View(model);
        }

        //POST: UnitsController/Create  //TODO 
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitDtoViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await IsDuplicateUnitAsync(model.CondominiumId, model.Floor, model.Door))
            {
                _flashMessage.Danger("A unit with the same floor and door already exists in this condominium.");
                return View(model);
            }


            var unitDto = _converterHelper.ToUnitDto(model);


            try
            {

                var result = await _apiCallService.PostAsync<UnitDto, Response<UnitDto>>("api/Units", unitDto);

                if (result.IsSuccess)
                {

                    if (model.MemberId != null)
                    {
                        var createdUnit = result.Results;

                        if (createdUnit == null)
                        {
                            return NotFound();
                        }

                        await _apiCallService.PostAsync<object, Response<object>>(
                         $"api/CondoMembers/{model.MemberId}/Units/{createdUnit.Id}",
                         new { } // Associa a unit com o member
                        );
                    }

                    if (model.MemberId != null)
                    {
                        return RedirectToAction("MemberUnits", new { id = model.MemberId });
                    }
                    if (model.CondoName != null)
                    {
                        return RedirectToAction(nameof(Index), new { id = model.CondominiumId, condoName = model.CondoName });
                    }

                    return RedirectToAction(nameof(Index));
                }


                _flashMessage.Danger(result.Message);
                model.CondoId = model.CondominiumId; // Ensure CondoId is set for the view model
                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Erro posting unit{ex.InnerException}");
                model.CondoId = model.CondominiumId;
                return View(model);
            }

        }

        // GET: UnitsController/Edit/5
        public async Task<ActionResult> Edit(int? id, int? condoId, string? condoName, int? memberId)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var unit = await _apiCallService.GetAsync<UnitDto>($"api/Units/{id.Value}");
                if (unit == null)
                {
                    return NotFound();
                }

                if (condoId != null && condoName != null)
                {
                    ViewBag.CondominiumId = condoId;
                    ViewBag.CondoName = condoName;

                }
                if (memberId != null)
                {
                    ViewBag.MemberId = memberId;
                }

                return View(unit);
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error retrieving unit for edit.");

                if (condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                }

                if (memberId != null)
                {
                    return RedirectToAction(nameof(MemberUnits), new { id = memberId });
                }

                return RedirectToAction(nameof(Index));
            }



        }



        // POST: UnitsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UnitDto unitDto, int? condoId, string? condoName, int? memberId)
        {
            if (id != unitDto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                if (condoId != null && condoName != null)
                {
                    ViewBag.CondominiumId = condoId;
                    ViewBag.CondoName = condoName;
                }
  
                if (memberId != null)
                {
                    ViewBag.MemberId = memberId;
                }

                return View(unitDto);
            }

            try
            {
  
                var result = await _apiCallService.PostAsync<UnitDto, Response<object>>($"api/Units/Edit/{id}", unitDto);

                if (!result.IsSuccess)
                {
                    _flashMessage.Danger("Error updating unit");
                    if (condoId != null && condoName != null)
                    {
                        ViewBag.CondominiumId = condoId;
                        ViewBag.CondoName = condoName;
                    }

                    if (memberId != null)
                    {
                        ViewBag.MemberId = memberId;
                    }

                    return View(unitDto);
                }


                if (condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName});
                }

                if (memberId != null)
                {
                    return RedirectToAction(nameof(MemberUnits), new { id = memberId });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("Error updating unit");
            }

            if (condoId != null && condoName != null)
            {
                ViewBag.CondominiumId = condoId;
                ViewBag.CondoName = condoName;
            }

            if (memberId != null)
            {
                ViewBag.MemberId = memberId;
            }

            return View(unitDto);
        }


        // POST: UnitsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, int? condoId, string? condoName)
        {
            if (id == null)
            {
                return NotFound();
            }
                

            try
            {
                var unit = await _apiCallService.GetAsync<UnitDto>($"api/Units/{id}");
                if (unit == null)
                {
                    _flashMessage.Danger("Unit not found.");

                    if(condoId != null && condoName != null)
                    {
                        return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                    }

                    return RedirectToAction(nameof(Index));

                }


                var hasRelations = unit.CondoMemberDtos?.Any() ?? false;
                if (hasRelations)
                {
                  

                    _flashMessage.Danger("Unit cannot be deleted because it is associated with one or more members.");

                    if (condoId != null && condoName != null)
                    {
                        return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                    }

                    return RedirectToAction(nameof(Index));
                }

                var result = await _apiCallService.DeleteAsync($"api/Units/{id}");
                if (!result.IsSuccessStatusCode)
                {
                    _flashMessage.Danger("Failed to delete unit. Please try again.");
                }
                else
                {
                    _flashMessage.Confirmation("Unit deleted successfully.");
                }


                if (condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                }


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _flashMessage.Danger("An unexpected error occurred.");

                if (condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                }

                return RedirectToAction(nameof(Index));
            }
        }




        public async Task<IActionResult> AssignMember(int? id, int? condoId, string? condoName)
        {

            if (id == null)
            {
                return NotFound();
            }
            if(condoId != null && condoName != null)
            {
                ViewBag.CondominiumId = condoId;
                ViewBag.CondoName = condoName;
            }

            try
            {
                var members = await _apiCallService.GetAsync<List<CondoMemberDto>>($"api/CondoMembers");
                if (members == null)
                {
                    return NotFound();
                }               


                var model = new AssignMemberViewModel
                {
                    Members = members,
                    UnitId = id.Value
                };

                model.Unit = await _apiCallService.GetAsync<UnitDto>($"api/Units/{id.Value}");


                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("Error retrieving members.");

                if (condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                }

                return RedirectToAction(nameof(Index));
            }


        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMember(AssignMemberViewModel model, int? condoId, string? condoName)
        {

            if (!ModelState.IsValid)
            {
                if (condoId != null && condoName != null)
                {
                    ViewBag.CondominiumId = condoId;
                    ViewBag.CondoName = condoName;
                }

                return View(model);
            }




            try
            {
                model.Unit = await _apiCallService.GetAsync<UnitDto>($"api/Units/{model.UnitId}");

                model.MemberIds ??= new List<int>();

                if (model.Unit.CondoMemberDtos != null)
                {
                    List<int> toDelete = new List<int>();


                    // percorre todos os membros já associados à Unit
                    foreach (var memberId in model.Unit.CondoMemberDtos.Select(c => c.Id))
                    {
                        // se esse id NÃO está nos selecionados no form
                        if (!model.MemberIds.Contains(memberId))
                        {
                            toDelete.Add(memberId); // adiciona à lista de remoção
                        }
                    }

                    foreach (var memberId in toDelete)
                    {
                        await _apiCallService.DeleteAsync($"api/CondoMembers/{memberId}/Units/{model.UnitId}");
                    }
                }
               

                foreach (var memberId in model.MemberIds)
                {
                    await _apiCallService.PostAsync<object, Response<object>>(
                        $"api/CondoMembers/{memberId}/Units/{model.UnitId}",
                        new { }
                    );
                }


               

                _flashMessage.Confirmation("Members updated succesfully.");

                if(condoId != null && condoName != null)
                {
                    return RedirectToAction(nameof(Index), new { id = condoId, condoName = condoName });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _flashMessage.Danger("Error updating member");

                if (condoId != null && condoName != null)
                {
                    ViewBag.CondominiumId = condoId;
                    ViewBag.CondoName = condoName;
                }

                return View(model);
            }

            

        }

        public async Task<IActionResult> UnitsFromCondo(int? condoId, string? condoName)
        {
            if(condoId == null|| condoName == null)
            {
                return NotFound();
            }

            ViewBag.CondoName = condoName;

            var units = new List<UnitDto>();

            try
            {
                units = await _apiCallService.GetAsync<List<UnitDto>>($"api/Units/condo/{condoId.Value}");

                return View(units);

            }

            catch (Exception ex)
            {
                _flashMessage.Danger($"Error fetching units");
                return View(units);
            }




        }




        private async Task<bool> IsDuplicateUnitAsync(int condoId, string floor, string door)
        {
            var existingUnits = await _apiCallService.GetAsync<IEnumerable<UnitDto>>($"api/Units/condo/{condoId}");

            return existingUnits.Any(u => u.Floor == floor && u.Door.Equals(door, StringComparison.OrdinalIgnoreCase));
        }

    }
}
