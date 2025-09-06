//using ClassLibrary;
//using ClassLibrary.DtoModels;
//using CondoManagementWebApp.Helpers;
//using CondoManagementWebApp.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;
//using Vereyon.Web;

//namespace CondoManagementWebApp.Controllers
//{
//    public class MeetingController : Microsoft.AspNetCore.Mvc.Controller
//    {
//        private readonly IApiCallService _apiCallService;
//        private readonly IFlashMessage _flashMessage;
//        private readonly IConverterHelper _converterHelper;

//        public MeetingController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
//        {
//            _apiCallService = apiCallService;
//            _flashMessage = flashMessage;
//            _converterHelper = converterHelper;
//        }
//        // GET: MeetingController
//        public async  Task<ActionResult<List<MeetingDto>>> IndexMeetings()
//        {
//            try
//            {
//                if (this.User.IsInRole("CondoManager"))
//                {
//                    var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
//                    if (condoManagerCondo == null)
//                    {
//                        _flashMessage.Danger("You are not managing any condos currently");
//                        return View(new List<MeetingDto>());
//                    }

//                    var meetingsDtos = await _apiCallService.GetAsync<List<MeetingDto>>($"api/Meeting/GetCondoMeetings/{condoManagerCondo.Id}");

//                    return View(meetingsDtos);
//                }
//                else if (this.User.IsInRole("CondoMember"))
//                {
//                    var meetingsDtos = await _apiCallService.GetAsync<List<MeetingDto>>($"api/Meeting/GetCondoMemberMeetings/{this.User.Identity.Name}");

//                    return View(meetingsDtos);
//                }

//                return View(new List<MeetingDto>());
//            }
//            catch
//            {
//                return View("Error");
//            }
            
//        }

//        // GET: MeetingController/Details/5
//        public async Task<ActionResult<MeetingDto>> DetailsMeeting(int id)
//        {
//            try
//            {
//                var meetingDto = await _apiCallService.GetAsync<MeetingDto>($"api/Meeting/GetMeeting/{id}");

//                if (meetingDto == null)
//                {
//                    _flashMessage.Danger("Unable to retrieve meeting");
//                    return View(new MeetingDto());
//                }

//                return View(meetingDto);
//            }
//            catch 
//            {

//                return View("Error");
//            }  
//        }

//        // GET: MeetingController/Create
//        [Authorize(Roles ="CondoManager")]
//        public async  Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateMeeting()
//        {
//            try
//            {
//                var condoManagerCondo = await _apiCallService.GetByQueryAsync<CondominiumDto>("api/Condominiums/GetCondoManagerCondominiumDto", this.User.Identity.Name);
//                if (condoManagerCondo == null)
//                {
//                    _flashMessage.Danger("You are not managing any condos currently");
//                    return View(new List<MeetingDto>());
//                }

//                var condoMembersList = await _apiCallService.GetAsync<List<CondoMemberDto>>($"api/CondoMembers/ByCondo/{condoManagerCondo.Id}") ?? new List<CondoMemberDto>();

//                var selectListCondoMembers = _converterHelper.ToCondoMembersSelectList(condoMembersList);

//                var occurrencesList = await _apiCallService.GetAsync<List<OccurrenceDto>>($"api/Occurrence/GetAllCondoOccurrences/{condoManagerCondo.Id}");

//                var selectListOccurrrences = _converterHelper.ToOccurrencesSelectList(occurrencesList);

//                var model = new CreateMeetingViewModel()
//                {
//                    CondominiumId = condoManagerCondo.Id,
//                    CondoMembersToSelect = selectListCondoMembers,
//                    OccurrencesToSelect = selectListOccurrrences,
//                };
//                return View(model);
//            }
//            catch 
//            {

//                return View("Error");
//            }
//        }

//        // POST: MeetingController/Create
//        [Microsoft.AspNetCore.Mvc.HttpPost]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> RequestCreateMeeting(CreateMeetingViewModel model)
//        {
//            try
//            {
//                var meetingDto = _converterHelper.ToNewMeetingDto(model);
                
//                //buscar listas com itens selecionados

//                var selectedCondomembersDto = await _apiCallService.PostAsync<List<int>, List<CondoMemberDto>>("api/Meeting/GetSelectedCondoMembers", model.SelectedCondoMembersIds);

//                var selectedOccurrencesDto = await _apiCallService.PostAsync<List<int>, List<OccurrenceDto>>("api/Meeting/GetSelectedOccurrences", model.SelectedOccurrencesIds);

//                //atribuir listas à meeting

//                meetingDto.OccurencesDto = selectedOccurrencesDto;
//                meetingDto.CondoMembersDto = selectedCondomembersDto;

//                //criar link
//                var jitsiRoomId = Guid.NewGuid().ToString().Replace("-", "");
//                meetingDto.MeetingLink = "https://meet.jit.si/" + jitsiRoomId;

//                //criar Voting e modificar view model (se der tempo)

//                //post Meeting

//                var apiCall = await _apiCallService.PostAsync<MeetingDto, Response<object>>("api/Meeting/CreateMeeting", meetingDto);

//                if(apiCall.IsSuccess)
//                {
//                    return RedirectToAction(nameof(IndexMeetings));
//                }
//                else
//                {
//                    _flashMessage.Danger(apiCall.Message);  
//                    return View("CreateMeeting", model);
//                }

                
//            }
//            catch
//            {
//                return View("Error");
//            }
//        }

//        //VOTING SE DER TEMPO
//        //// GET: MeetingController/Edit/5
//        //public ActionResult Edit(int id)
//        //{
//        //    return View();
//        //}

//        //// POST: MeetingController/Edit/5
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public ActionResult Edit(int id, IFormCollection collection) // voting se for possivel
//        //{
//        //    try
//        //    {
//        //        return RedirectToAction(nameof(Index));
//        //    }
//        //    catch
//        //    {
//        //        return View();
//        //    }
//        //}


        

//        // POST: MeetingController/Delete/5    Cancel Meeting
//        [Microsoft.AspNetCore.Mvc.HttpPost]
//        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DeleteMeeting(int id)
//        {
//            try
//            {

//                var apiCall = await _apiCallService.PostAsync<int, Response<object>>($"api/Meeting/DeleteMeeting", id);
//                if (apiCall.IsSuccess)
//                {
//                    return RedirectToAction(nameof(IndexMeetings));
//                }
//                else
//                {
//                    _flashMessage.Danger(apiCall.Message);
//                    return RedirectToAction(nameof(IndexMeetings));
//                }
//            }
//            catch
//            {
//                return View("Error");
//            }
//        }
//    }
//}
