using ClassLibrary;
using ClassLibrary.DtoModels;
using CondoManagementWebApp.Helpers;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Threading.Tasks;
using Vereyon.Web;

namespace CondoManagementWebApp.Controllers
{
    
    public class MessageController : Controller
    {
        private readonly IApiCallService _apiCallService;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper; 

        public MessageController(IApiCallService apiCallService, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _apiCallService = apiCallService;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }


        // GET: Message
        public async Task<IActionResult> IndexReceived()
        {
            try
            {
                var messagesDto = await _apiCallService.GetAsync<List<MessageDto>>("api/Message/GetAllMessages");

                var receivedMessagesDto = new List<MessageDto>();  

                foreach (var messageDto in messagesDto)
                {
                    if (messageDto.DeletedByReceiver == false && messageDto.ReceiverEmail == User.Identity.Name)
                    {
                        receivedMessagesDto.Add(messageDto);
                    }
                }

                return View(receivedMessagesDto);
            }
            catch
            {
                return View("Error");
            }
            
        }


        // GET: Message
        public async Task<IActionResult> IndexSent()
        {
            try
            {
                var messagesDto = await _apiCallService.GetAsync<List<MessageDto>>("api/Message/GetAllMessages");

                var receivedMessagesDto = new List<MessageDto>();

                foreach (var messageDto in messagesDto)
                {
                    if (messageDto.DeletedBySender == false && messageDto.SenderEmail == User.Identity.Name)
                    {
                        receivedMessagesDto.Add(messageDto);
                    }
                }

                return View(receivedMessagesDto);
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

        public async Task<ActionResult> RequestCreateMessages(CreateMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _flashMessage.Danger("Unable to send message, due to system error");
                return View("CreateMessage", model);   
            }

            try
            {
                var user = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUserByEmail", model.ReceiverEmail);

                if (user == null)
                {
                    _flashMessage.Danger("Unable to send message. No user was found under this email");
                    return View("CreateMessage", model);
                }

                //Popular propriedades de message dto fora do model

                DateTime date = DateTime.Now;

                string senderEmail = this.User.Identity.Name;

                var status = new EnumDto() { Value = 1, Name = "Unresolved" };

                var messageDto = _converterHelper.ToMessageDto(model, date, senderEmail, status);

                var apiCall = await _apiCallService.PostAsync<MessageDto, Response<object>>("api/Message/CreateMessage", messageDto);

                if (apiCall.IsSuccess)
                {
                    //Enviar notificação via email para quem recebe mensagem 
                    var sendEmail = await _apiCallService.PostAsync<string, Response<object>>("api/Message/SendEmailNotification", messageDto.ReceiverEmail);

                    if (sendEmail.IsSuccess)
                    {
                        return RedirectToAction(nameof(IndexSent));
                    }

                    _flashMessage.Warning(sendEmail.Message);
                    return View("CreateMessage", model);
                }

                _flashMessage.Danger($"{apiCall.Message}");

                return View("CreateMessage", model);
            }
            catch (HttpRequestException ex)
            {
                // Captura a exceção específica para requisições HTTP
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                   
                    _flashMessage.Danger("Unable to send message. No user was found under this email.");
                }
                else
                {
                 
                    _flashMessage.Danger("Unable to send message, due to a network error.");
                }
                return View("CreateMessage", model);
            }
            catch
            {
                _flashMessage.Danger("Unable to send message, due to unexpected error");
                return View("CreateMessage", model);
            }
            
        }

       

        // GET: MessageController/MessageDetails/5
        public async Task<IActionResult> MessageDetails(int id)
        {
            try
            {
                //buscar mensagem com lista de status
                var messageDto = await _apiCallService.GetAsync<MessageDto>($"api/Message/MessageDetails/{id}");
                
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
        public async Task<ActionResult> EditMessageStatus(MessageDto messageDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var messageDtoWithStatusName = await SelectStatusName(messageDto);

                    var apiCall = await _apiCallService.PostAsync<MessageDto,Response<object>>("api/Message/EditMessageStatus", messageDtoWithStatusName);

                    if (apiCall.IsSuccess)
                    {
                        _flashMessage.Confirmation($"{apiCall.Message}");

                        var editedMessage = await _apiCallService.GetAsync<MessageDto>($"api/Message/MessageDetails/{messageDto.Id}");

                        return View("MessageDetails", editedMessage);
                    }

                    _flashMessage.Danger($"{apiCall.Message}");
                    return View("MessageDetails", messageDto);
                }
                catch
                {
                    _flashMessage.Danger($"Unable to change message status due to error");
                    return View("MessageDetails", messageDto);
                }
            }
            else
            {
                _flashMessage.Danger($"Unable to change message status due to error");
                return View("MessageDetails",  messageDto);  
            }
           
        }

        public async Task<MessageDto> SelectStatusName(MessageDto messageDto)
        {
            //Fazer seleção do Name do status (a select list só preenche o value)
            var statusList = await _apiCallService.GetAsync<List<SelectListItem>>("api/Message/GetMessageStatusList");

            var selectedStatus = statusList.FirstOrDefault(s => s.Value == messageDto.Status.Value.ToString());

            if (selectedStatus != null)
            {
                // Preencher Name do EnumDto antes de enviar para a API
                messageDto.Status.Name = selectedStatus.Text;
            }

            return messageDto;
        }



        // GET: MessageController/DeleteSentMessges/5
        [HttpPost]
        public async Task<ActionResult> DeleteReceivedMessages(int id)
        {
           

            try
            {
                var apiCall = await _apiCallService.PostAsync<int, Response<object>>($"api/Message/DeleteReceivedMessages", id);

                return Json(new { success = apiCall.IsSuccess });
            }
            catch
            {
                return Json(new { success = false, message = "An HTTP error occurred. Please try again later." });
            }
        }

        // POST: MessageController/DeleteSentMessages/5
        [HttpPost]
        public async Task<ActionResult> DeleteSentMessages(int id)
        {
            try
            {
                
                var apiCall = await _apiCallService.PostAsync<int, Response<object>>($"api/Message/DeleteSentMessages", id);

                // Retornar o resultado da API diretamente como um JSON para o AJAX processar.
                return Json(new { success = apiCall.IsSuccess });
            }
            catch
            {
                return Json(new { success = false, message = "An HTTP error occurred. Please try again later." });
            }
        }
    }
}
