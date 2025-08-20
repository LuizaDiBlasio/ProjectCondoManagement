using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    public class MessageController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;

        public MessageController(IApiCallService apiCallService, IFlashMessage flashMessage)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
        }


        // GET: Message
        public async Task<IActionResult> Index()
        {
            try
            {
                var messages = await _apiCallService.GetAsync<List<MessageDto>>("api/GetAllMessages");

                return View(messages);
            }
            catch
            {
                return View("Error");
            }
            
        }

        // GET: CreateMessage
        public ActionResult CreateMessage()
        {
            
            return View();
        }

        public async Task<ActionResult> RequestCreateMessages(MessageDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateMessage", messageDto);   
            }

            messageDto.SenderEmail = this.User.Identity.Name;
            messageDto.PostingDate = DateTime.Now;
            messageDto.Status = new EnumDto() {Value = 1, Name = "Unresolved" };

            var apiCall = await _apiCallService.PostAsync<MessageDto, Response>("api/Message/CreateMessage", messageDto);

            if (apiCall.IsSuccess)
            {
                return RedirectToAction("Index");   
            }

            _flashMessage.Danger($"{apiCall.Message}");

            return View("CreateMessage", messageDto);
        }

        public ActionResult SentMessages()
        {
            return View();
        }

        // GET: MessageController/Details/5
        public async Task<IActionResult> MessageDetails(int id)
        {
            try
            {
                //buscar mensagem com lista de status
                var messageDto = await _apiCallService.GetAsync<MessageDto>($"api/Message/MessaDetais{id}");
                
                if(messageDto == null)
                {
                    return View("NotFound");
                }

                return View(messageDto);
            }
            catch
            {
                return View("Error");
            }
            
        }

        // POST: MessageController/Edit/5
        [HttpPost]
        public ActionResult ChangeMessageDetails(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MessageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MessageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
