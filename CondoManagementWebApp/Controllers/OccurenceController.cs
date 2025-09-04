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
        public async Task<ActionResult<List<OccurrenceDto>>> IndexOccurrences()
        {
            try
            {
                if (this.User.IsInRole("CondoManager"))
                {
                    var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
                    if (condoManagerCondo == null)
                    {
                        _flashMessage.Danger("You are not managing any condos currently");
                        return View(new List<OccurrenceDto>());
                    }

                    var condoOccurences = await _apiCallService.GetAsync<List<OccurrenceDto>>($"api/Occurrence/GetAllCondoOccurrences/{condoManagerCondo.Id}");

                    if (!condoOccurences.Any())
                    {
                        return View(new List<OccurrenceDto>());
                    }

                    return View(condoOccurences);
                }
                else
                {
                    var condoMemberOccurrences = await _apiCallService.GetAsync<List<OccurrenceDto>>($"api/Occurrence/GetCondoMemberOccurrences/{this.User.Identity.Name}");

                    return View(condoMemberOccurrences);
                }
               
            }
            catch
            {
                return View("Error");
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
                return View("Error");  
            }
            

        }

        // GET: OccurenceController/Create
        public async Task<ActionResult> CreateOccurrence()
        {
            var model = new CreateOccurrenceViewModel();

            //achar o condominio do condoManager
            if (this.User.IsInRole("CondoManager"))
            {
                model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);
            }
            else //será o condoMember
            {
                model.UnitsToSelect = await GetCondoMemberUnitsList(this.User.Identity.Name);
            }

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
                    if (this.User.IsInRole("CondoManager"))
                    {
                        model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);
                    }
                    else //será o condoMember
                    {
                        model.UnitsToSelect = await GetCondoMemberUnitsList(this.User.Identity.Name);
                    }

                    return View("CreateOccurrence", model);
                }

                var occurrenceDto = _converterHelper.ToOccurenceDto(model);

                //obter lista de units selecionadas
                var selectedUnitsList = await _apiCallService.PostAsync<List<int>, List<UnitDto>>("api/Occurrence/GetSelectedUnits", model.SelectedUnitIds);

                occurrenceDto.UnitDtos = selectedUnitsList;

                //atribuir condo
                if (this.User.IsInRole("CondoManager"))
                {
                    var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);

                    occurrenceDto.CondominiumId = condoManagerCondo.Id;
                }
                else
                {
                    var condoMemberWithUnits =  await _apiCallService.GetAsync<CondoMemberDto>($"api/CondoMembers/ByEmail/{this.User.Identity.Name}");

                    var firstSelectedUnit = selectedUnitsList.FirstOrDefault(); //seleciona primeiro item escolhido da lista, uma ocorrencia não pode acontecer em 2 condos diferentes

                    if (firstSelectedUnit != null)
                    {
                        occurrenceDto.CondominiumId = firstSelectedUnit.CondominiumId;
                    }

                }


                var apicall = await _apiCallService.PostAsync<OccurrenceDto, Response<object>>("api/Occurrence/CreateOccurrence", occurrenceDto);

                if (apicall.IsSuccess)
                {
                    if (this.User.IsInRole("CondoManager"))
                    {
                        return RedirectToAction(nameof(IndexOccurrences));
                    }
                    else
                    {
                        _flashMessage.Confirmation("Occurence reported successfully, condominium administratio will contact you as soon as possible");
                        model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);
                        return View("CreateOccurrence", model);
                    }
                }
                else
                {
                    _flashMessage.Danger(apicall.Message);
                    model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);
                    return View("CreateOccurrence", model);
                }
            }
            catch
            {
                return View("Error");
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

                List<int> selectedIds = occurrenceDto.UnitDtos.Select(u => u.Id).ToList();

                var model = _converterHelper.ToEditOccurrenceView(occurrenceDto, selectedIds);
               
                //carregar listas

                if (this.User.IsInRole("CondoManager"))
                {
                    model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);
                }
                else //será o condoMember
                {
                    model.UnitsToSelect = await GetCondoMemberUnitsList(this.User.Identity.Name);
                }

                return View(model);
            }
            catch
            {
                return View("Error");
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
                    
                   model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);
                    
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
                    model.UnitsToSelect = await GetUnitsList(this.User.Identity.Name);

                    return View("EditOccurrence", model);
                }
                
            }
            catch
            {
                return View("Error");
            }
        }

        //Metodos Auxiliares
        public async Task<List<SelectListItem>> GetUnitsList(string condoManagerEmail)
        {
            var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", condoManagerEmail);
            if (condoManagerCondo == null)
            {    
                return new List<SelectListItem>();
            }

            return await _apiCallService.GetAsync<List<SelectListItem>>($"api/Units/GetCondoUnitsList/{condoManagerCondo.Id}");
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
        
    }
}
