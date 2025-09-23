using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Syncfusion.EJ2.Layouts;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    [Route("[controller]")]
    public class MeetingController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;

     
        public MeetingController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }


        // GET: MeetingController
        [HttpGet("IndexMeetings")]
        public async Task<ActionResult<List<CondominiumWithMeetingsDto>>> IndexMeetings()
        {
            try
            {
                if (this.User.IsInRole("CondoManager"))
                {
                    var condosWithMeetingsDtos = await _apiCallService.GetAsync<List<CondominiumWithMeetingsDto>>($"api/Meeting/GetCondosWithMeetings");
                    if (!condosWithMeetingsDtos.Any())
                    {
                        return View(new List<CondominiumWithMeetingsDto>());
                    }

                    return View(condosWithMeetingsDtos);
                }
                else if (this.User.IsInRole("CondoMember"))
                {
                    var condosWithMeetingsDtos = await _apiCallService.GetAsync<List<CondominiumWithMeetingsDto>>($"api/Meeting/GetCondoMemberMeetings/{this.User.Identity.Name}");

                    return View(condosWithMeetingsDtos);
                }

                return View(new List<CondominiumWithMeetingsDto>());
            }
            catch
            {
                return View("Error500");
            }

        }

        // GET: MeetingController/Details/5
        [HttpGet("DetailsMeeting/{id}")]
        public async Task<ActionResult<MeetingDetailsViewModel>> DetailsMeeting(int id)
        {
            try
            {
                var model = new MeetingDetailsViewModel();

                var meetingDto = await _apiCallService.GetAsync<MeetingDto>($"api/Meeting/GetMeetingWithMembersAndOccurrences/{id}");

                if (meetingDto == null)
                {
                    _flashMessage.Danger("Unable to retrieve meeting");

                    return RedirectToAction(nameof(IndexMeetings));
                }

                var condominium = await _apiCallService.GetAsync<CondominiumDto>($"api/Condominiums/{meetingDto.CondominiumId}");

                if(condominium == null)
                {
                    _flashMessage.Danger("Unable to retrieve meeting");

                    return RedirectToAction(nameof(IndexMeetings));
                }

                model.CondominiumDto = condominium;
                model.MeetingDto = meetingDto;  

                return View(model);
            }
            catch
            {

                return View("Error500");
            }
        }

        // GET: MeetingController/Create
        [HttpGet("CreateMeeting")]
        [Authorize(Roles = "CondoManager")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateMeeting()
        {
            try
            {
                var managerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");
                if (managerCondos == null)
                {
                    _flashMessage.Danger("You are not managing any condos currently");
                    return RedirectToAction(nameof(IndexMeetings));
                }

                var managerCondosList = _converterHelper.ToCondosSelectList(managerCondos);

                var model = new CreateMeetingViewModel()
                {
                    CondosToSelect = managerCondosList,
                };
                return View(model);
            }
            catch
            {

                return View("Error500");
            }
        }


        // chamadas ajax

        //para condomembers list
        [Microsoft.AspNetCore.Mvc.HttpGet("GetCondoMembersFromCondo/{id}")]
        public async Task<List<SelectListItem>> GetCondoMembersFromCondo(int id)
        {
            var condoMembersList = await _apiCallService.GetAsync<List<CondoMemberDto>>($"api/CondoMembers/ByCondo/{id}") ?? new List<CondoMemberDto>();

            var selectListCondoMembers = _converterHelper.ToCondoMembersSelectList(condoMembersList);

            return (selectListCondoMembers);
        }


        //para occurrences list
        [Microsoft.AspNetCore.Mvc.HttpGet("GetOccurencesFromCondo/{id}")]
        public async Task<List<SelectListItem>> GetOccurencesFromCondo(int id)
        {
            var occurrencesList = await _apiCallService.GetAsync<List<OccurrenceDto>>($"api/Occurrence/GetCondoOccurrences/{id}");

            var selectListOccurrrences = _converterHelper.ToOccurrenceSelectList(occurrencesList);

            return (selectListOccurrrences);
        }


        // POST: MeetingController/Create
        [Microsoft.AspNetCore.Mvc.HttpPost("RequestCreateMeeting")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RequestCreateMeeting(CreateMeetingViewModel model)
        {
            

            try
            {
                if (!ModelState.IsValid)
                {
                    var managerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");

                    var managerCondosList = _converterHelper.ToCondosSelectList(managerCondos);

                    model.CondosToSelect = managerCondosList;   

                    return View("CreateMeeting", model);
                }

                var meetingDto = _converterHelper.ToNewMeetingDto(model);

                //buscar listas com itens selecionados

                var selectedCondomembersDto = await _apiCallService.PostAsync<List<int>, List<CondoMemberDto>>("api/Meeting/GetSelectedCondoMembers", model.SelectedCondoMembersIds);

                if(model.SelectedOccurrencesIds != null)
                {
                    model.SelectedOccurrencesIds.RemoveAll(id => id == 0);
                }

                var selectedOccurrencesDto = await _apiCallService.PostAsync<List<int>, List<OccurrenceDto>>("api/Meeting/GetSelectedOccurrences", model.SelectedOccurrencesIds);

                //atribuir listas à meeting

                meetingDto.OccurencesDto = selectedOccurrencesDto;
                meetingDto.CondoMembersDto = selectedCondomembersDto;

                //criar link
                var jitsiRoomId = Guid.NewGuid().ToString().Replace("-", "");
                meetingDto.MeetingLink = "https://meet.jit.si/" + jitsiRoomId;

                //criar Voting e modificar view model (se der tempo)

                //post Meeting

                var apiCall = await _apiCallService.PostAsync<MeetingDto, Response<object>>("api/Meeting/CreateMeeting", meetingDto);

                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexMeetings));
                }
                else
                {
                    _flashMessage.Danger(apiCall.Message);
                    return View("CreateMeeting", model);
                }


            }
            catch
            {
                return View("Error500");
            }
        }


        // GET: MeetingController/Edit/5
        [HttpGet("EditMeeting/{id}")]
        public async Task<ActionResult> EditMeeting(int id)
        {

            try
            {
                var meetingDto = await _apiCallService.GetAsync<MeetingDto>($"api/Meeting/GetMeetingWithMembersAndOccurrences/{id}");

                if (meetingDto == null)
                {
                    _flashMessage.Danger("Unable to modify, meeting not founf in the system.");
                    var modelEmpty = new EditMeetingViewModel();
                    return View(modelEmpty);
                }

                //buscar meeting
                var model = _converterHelper.ToEditMeetingViewModel(meetingDto);


                //Mandar listas  para o model

                var occurrencesList = await GetOccurencesFromCondo(meetingDto.CondominiumId);

                model.OccurrencesToSelect = occurrencesList;

                var condmembersList = await GetCondoMembersFromCondo(meetingDto.CondominiumId);

                model.CondoMembersToSelect = condmembersList;

                var managerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");

                var managerCondosList = _converterHelper.ToCondosSelectList(managerCondos);

                model.CondosToSelect = managerCondosList;


                //buscar os ids selecionados

                model.SelectedCondoMembersIds = meetingDto.CondoMembersDto.Select(cm => cm.Id).ToList();    

                model.SelectedOccurrencesIds = meetingDto.OccurencesDto.Select(o => o.Id).ToList();

                return View(model);

            }
            catch
            {
                return View("Error500");
            }
           

        }

        // POST: MeetingController/Edit/5
        [HttpPost("RequestEditMeeting")]
        public async Task<ActionResult> RequestEditMeeting(EditMeetingViewModel model) 
        {
            try
            {
                var meetingDto = await _apiCallService.GetAsync<MeetingDto>($"api/Meeting/GetMeetingWithMembersAndOccurrences/{model.Id}");
                if(meetingDto == null)
                {
                    _flashMessage.Danger("Meeting not found in the system, unable to modify");
                    return View("EditMeeting", model);
                }

                if (!ModelState.IsValid)
                {
                    await LoadLists(model, meetingDto);

                    return View("EditMeeting", model);
                }

                //buscar listas com itens selecionados

                var selectedCondomembersDto = await _apiCallService.PostAsync<List<int>, List<CondoMemberDto>>("api/Meeting/GetSelectedCondoMembers", model.SelectedCondoMembersIds);

                if (model.SelectedOccurrencesIds != null)
                {
                    model.SelectedOccurrencesIds.RemoveAll(id => id == 0);
                }

                var selectedOccurrencesDto = await _apiCallService.PostAsync<List<int>, List<OccurrenceDto>>("api/Meeting/GetSelectedOccurrences", model.SelectedOccurrencesIds);

                //atribuir propriedades
                _converterHelper.SetEditedMeetingProperties(meetingDto, selectedCondomembersDto, selectedOccurrencesDto, model);
                
     
                var apiCall = await _apiCallService.PostAsync<MeetingDto, Response<object>>("api/Meeting/EditMeeting", meetingDto);
                if (apiCall.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexMeetings));
                }
                else
                {
                    _flashMessage.Danger(apiCall.Message);
                    return View("EditMeeting", model);
                }
            }
            catch
            {
                return View("Error500");
            }
        }




        //  DELETE: MeetingController/Delete/5    Cancel Meeting
        [Microsoft.AspNetCore.Mvc.HttpDelete("RequestDeleteMeeting/{id}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RequestDeleteMeeting(int id)
        {
            try
            {
                var apiCall = await _apiCallService.DeleteAsync($"api/Meeting/DeleteMeeting/{id}");

                if (apiCall.IsSuccessStatusCode)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    _flashMessage.Danger("Unable to cancel meeting due to server error");
                    return BadRequest(new { success = false, message = "Unable to cancel meeting due to server error " });
                }
            }
            catch
            {
                return View("Error500");
            }
        }

        //metodo auxiliar
        public async Task LoadLists(EditMeetingViewModel model, MeetingDto meetingDto)
        {
            if (!ModelState.IsValid)
            {
                var occurrencesList = await GetOccurencesFromCondo(meetingDto.CondominiumId);

                model.OccurrencesToSelect = occurrencesList;

                var condmembersList = await GetCondoMembersFromCondo(meetingDto.CondominiumId);

                model.CondoMembersToSelect = condmembersList;

                var managerCondos = await _apiCallService.GetAsync<List<CondominiumDto>>("api/Condominiums/ByManager");

                var managerCondosList = _converterHelper.ToCondosSelectList(managerCondos);

                model.CondosToSelect = managerCondosList;


                //buscar os ids selecionados

                model.SelectedCondoMembersIds = meetingDto.CondoMembersDto.Select(cm => cm.Id).ToList();

                model.SelectedOccurrencesIds = meetingDto.OccurencesDto.Select(o => o.Id).ToList();
            }
        }

       
    }
}
