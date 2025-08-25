using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Users;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        private readonly DataContextUsers _dataContextUsers;
        private readonly IConverterHelper _converterHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;


        public MessageController(IMessageRepository messageRepository, DataContextUsers contextUsers, IConverterHelper converterHelper, IConfiguration configuration, IMailHelper mailHelper)
        {
            _messageRepository = messageRepository;
            _dataContextUsers = contextUsers;
            _converterHelper = converterHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        //[AllowAnonymous]
        // GET: MessageController
        [HttpGet("GetAllMessages")]
        public ActionResult<IEnumerable<MessageDto>> GetAllMessages()
        {
            var messages = _messageRepository.GetAll(_dataContextUsers);

            // Se o repositório retornar null, converto para uma lista vazia
            var messagesList = messages ?? Enumerable.Empty<Message>();

            var messagesDto = messagesList.Select(m => _converterHelper.ToMessageDto(m, null)).ToList();

      
            return Ok(messagesDto);

        }

        // GET: MessageController/MessageDetails/5
        [HttpGet("MessageDetails/{id}")]
        public async Task<ActionResult> MessageDetails(int id)
        {
            var message = await _messageRepository.GetByIdAsync(id, _dataContextUsers);

            if (message == null)
            {
                return NotFound(new Response () { IsSuccess = false, Message = "Message not found, unable to retrieve details"});  
            }

            var statusList = _messageRepository.GetMessageStatusList();

            var messageDto = _converterHelper.ToMessageDto(message, statusList);    

            return Ok(messageDto); 

        }


        // POST: MessageController/Edit/5
        [HttpPost("EditMessageStatus")]
        public ActionResult EditMessageStatus([FromBody] MessageDto messageDto)
        {
            if (messageDto == null)
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable change message status" });
            }

            try
            {
                var message = _converterHelper.ToMessage(messageDto, false);

                _messageRepository.UpdateAsync(message, _dataContextUsers);

                return Ok(new Response() { IsSuccess = true, Message = "Status updated successfully" });
            }
            catch
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable change message status" });
            }
        }


        // POST: MessageController/CreateMessage
        [HttpPost("CreateMessage")]
        public async Task<ActionResult> Create([FromBody] MessageDto messageDto)
        {
            if(messageDto == null)
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable to send message" });
            }

            try
            {
                var message = _converterHelper.ToMessage(messageDto, true);
                
                await _messageRepository.CreateAsync(message, _dataContextUsers);

                return Ok(new Response() { IsSuccess = true , Message ="Message sent successfully!"});
            }
            catch
            {
                return BadRequest(new Response() { IsSuccess = false, Message = "Unable to send message" });
            }
        }



        // DELETE: MessageController/DeleteReceivedMessages
        [HttpPost("DeleteReceivedMessages")]
        public async Task<ActionResult> DeleteReceivedMessages([FromBody]int id)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(id, _dataContextUsers);

                if (message == null)
                {
                    return NotFound(new Response { IsSuccess = false, Message = "Unable to delete, message not found" });
                }

                message.DeletedByReceiver = true;

                await _messageRepository.UpdateAsync(message, _dataContextUsers);

                return Ok(new Response { IsSuccess = true });
            }
            catch
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Unable to delete message" });
            }
        }

        //DELETE: MessageController/DeleteSentMessages
        [HttpPost("DeleteSentMessages")]

        public async Task<ActionResult> DeleteSentMessages([FromBody]int id)
        {
            try
            {
                var message = await _messageRepository.GetByIdAsync(id, _dataContextUsers);

                if (message == null)
                {
                    return NotFound(new Response { IsSuccess = false, Message = "Unable to delete, message not found" });
                }

                message.DeletedBySender = true;

                await _messageRepository.UpdateAsync(message, _dataContextUsers);

                return Ok(new Response { IsSuccess = true});
            }
            catch
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Unable to delete message" });
            }
        }

        //METODO AUXILIAR   
        [HttpGet("GetMessageStatusList")]
        public List<SelectListItem> GetMessageStatusList()
        {
            return _messageRepository.GetMessageStatusList();
        }


        [HttpPost("SendEmailNotification")]
        public async Task<IActionResult> SendEmailNotification([FromBody] string email)
        {
            // gera um link para o login
            string link = $"{_configuration["WebAppSettings:BaseUrl"]}/Account/Login"; // garante que o token seja codificado corretamente mesmo com caracteres especiais

            Response response = _mailHelper.SendEmail(email, "Email notification", $"<h1>Email notification</h1>" +
           $"You have receveived a message in your Omah app,<br><br><a href = \"{link}\">Click here to login and check your inbox. </a>"); //Contruir email e enviá-lo com o link 

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            else
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Message sent internally but no notification was sent to receivers email" });
            }

        }

    }
}
