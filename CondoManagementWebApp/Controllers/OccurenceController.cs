using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class OccurrenceController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;

        public OccurrenceController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }
        // GET: OccurenceController
        [HttpGet("IndexOccurrences")]
        public async Task<ActionResult<List<CondominiumWithOccurrencesDto>>> IndexOccurrences()
        {
            try
            {
                if (this.User.IsInRole("CondoManager"))
                {
                   
                    var condoOccurences = await _apiCallService.GetAsync<List<CondominiumWithOccurrencesDto>>($"api/Occurrence/GetAllCondoOccurrences");

                    if (!condoOccurences.Any())
                    {
                        return View(new List<CondominiumWithOccurrencesDto>());
                    }

                    return View(condoOccurences);
                }
                else if (this.User.IsInRole("CondoMember"))
                {
                    var condoMemberOccurrences = await _apiCallService.GetAsync<List<CondominiumWithOccurrencesDto>>($"api/Occurrence/GetCondoMemberOccurrences/{this.User.Identity.Name}");

                    return View(condoMemberOccurrences);
                }
                return View(new List<CondominiumWithOccurrencesDto>());
            }
            catch
            {
                return View("Error500");
            }
            
        }

        // GET: OccurenceController/Details/5
        [HttpGet("DetatilsOccurrence/{id}")]
        public async Task<ActionResult> DetailsOccurrence(int id)
        {
            try
            {
                var occurrence = await _apiCallService.GetAsync<OccurrenceDto>($"api/Occurrence/GetOccurrenceWithUnits/{id}");
                if (occurrence == null)
                {
                    _flashMessage.Danger("Unable to retrieve occurrence");
                    return RedirectToAction(nameof(IndexOccurrences));
                }

                return View(occurrence);
            }
            catch
            {
                return View("Error500");  
            }
            

        }

        // GET: OccurenceController/Create
        public async Task<ActionResult> CreateOccurrence()
        {
            var model = new CreateOccurrenceViewModel();

            model.CondosToSelect = await GetCondosList();

            return View(model);

        }

        // POST: OccurenceController/Create
        [HttpPost]
        public async Task<ActionResult> RequestCreateOccurrence(CreateOccurrenceViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.CondosToSelect = await GetCondosList();

                    return View("CreateOccurrence", model);
                }

                var occurrenceDto = _converterHelper.ToOccurenceDto(model);

                if (model.SelectedUnitIds.Any())
                {
                    //obter lista de units selecionadas
                    var selectedUnitsList = await _apiCallService.PostAsync<List<int>, List<UnitDto>>("api/Occurrence/GetSelectedUnits", model.SelectedUnitIds);

                    occurrenceDto.UnitDtos = selectedUnitsList;
                }
               
                var apicall = await _apiCallService.PostAsync<OccurrenceDto, Response<object>>("api/Occurrence/CreateOccurrence", occurrenceDto);

                if (apicall.IsSuccess)
                {
                    if (this.User.IsInRole("CondoManager"))
                    {
                        _flashMessage.Confirmation("Occurence reported successfully!");
                        return RedirectToAction(nameof(IndexOccurrences));
                    }
                    else
                    {
                        _flashMessage.Confirmation("Occurence reported successfully, condominium administratio will contact you as soon as possible.");
                        return RedirectToAction(nameof(IndexOccurrences));
                    }
                }
                else
                {
                    _flashMessage.Danger(apicall.Message);
                    model.CondosToSelect = await GetCondosList();
                    return View("CreateOccurrence", model);
                }
            }
            catch
            {
                return View("Error500");
            }
        }

       
        // GET: OccurenceController/Edit/5
        public async Task<ActionResult> EditOccurrence(int id)
        {
            try
            {
                // buscar occurence
                var occurrenceDto = await _apiCallService.GetAsync<OccurrenceDto>($"api/Occurrence/GetOccurrenceWithUnits/{id}");
                if (occurrenceDto == null)
                {
                    _flashMessage.Danger("Unable to retrieve occurrence");
                    return RedirectToAction(nameof(IndexOccurrences));
                }

                var selectedIds = new List<int>();

                if(occurrenceDto.UnitDtos != null && occurrenceDto.UnitDtos.Any())
                {
                   selectedIds = occurrenceDto.UnitDtos.Select(u => u.Id).ToList();
                }
                
                var model = _converterHelper.ToEditOccurrenceView(occurrenceDto, selectedIds);

                model.CondosToSelect = await GetCondosList();

                if (model.CondominiumId != null)
                {
                    model.UnitsToSelect = await GetUnitsList(model.CondominiumId.Value);
                }

                return View(model);
            }
            catch
            {
                return View("Error500");
            }
           
        }

        // POST: OccurenceController/Edit/5
        [HttpPost]
        public async Task<ActionResult> RequestEditOccurrence(EditOccurrenceViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.CondosToSelect = await GetCondosList();

                    if (model.CondominiumId != null)
                    {
                        model.UnitsToSelect = await GetUnitsList(model.CondominiumId.Value);
                    }

                    return View("EditOccurrence", model);
                }

                var editedOccurrenceDto = _converterHelper.ToEditedOccurrenceDto(model);

                //obter lista de units selecionadas
                var selectedUnitsList = await _apiCallService.PostAsync<List<int>, List<UnitDto>>("api/Occurrence/GetSelectedUnits", model.SelectedUnitIds);

                editedOccurrenceDto.UnitDtos = selectedUnitsList;

                editedOccurrenceDto.IsResolved = model.IsResolved;

                if (editedOccurrenceDto.IsResolved)
                {
                    editedOccurrenceDto.ResolutionDate = DateTime.Now;  
                }

                var apiCall = await _apiCallService.PostAsync<OccurrenceDto, Response<object>>("api/Occurrence/EditOccurrence", editedOccurrenceDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexOccurrences));
                }
                else
                {
                    model.CondosToSelect = await GetCondosList();

                    if (model.CondominiumId != null)
                    {
                        model.UnitsToSelect = await GetUnitsList(model.CondominiumId.Value);
                    }

                    return View("EditOccurrence", model);
                }
                
            }
            catch
            {
                return View("Error500");
            }
        }

        //Metodos Auxiliares
        public async Task<List<SelectListItem>> GetUnitsList(int condoId)
        {

            return await _apiCallService.GetAsync<List<SelectListItem>>($"api/Units/GetCondoUnitsList/{condoId}");
        }

        public async Task<List<SelectListItem>> GetCondoMemberUnitsList(string condoMemberEmail)
        {
           
            var condoMemberDto = await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/ByEmail/{condoMemberEmail}");


            if(condoMemberDto.Units != null && condoMemberDto.Units.Any())
            {
                return condoMemberDto.Units.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"Door {u.Door.ToString()} - Floor {u.Floor.ToString()} - Condo {u.CondominiumDto.CondoName}"
                }).ToList();
            }
            return new List<SelectListItem>();
        }


        private async Task<List<SelectListItem>> GetCondosList()
        {
            var condosList = new List<SelectListItem>();    

            if (this.User.IsInRole("CondoManager"))
            {
                var managerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>($"api/Condominiums/ByManager");
                if (managerCondos != null && managerCondos.Any())
                {
                    condosList = managerCondos.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CondoName.ToString()
                    }).ToList();
                }
            }
            else if (this.User.IsInRole("CondoMember"))
            {
                var memberCondos = await _apiCallService.GetAsync<List<CondominiumDto>>($"api/Condominiums/ByCondoMember");
                if (memberCondos != null && memberCondos.Any())
                {
                    condosList = memberCondos.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.CondoName.ToString()
                    }).ToList();
                }
            }

            return condosList;

        }


    }
}
