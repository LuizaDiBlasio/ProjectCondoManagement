using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Users;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        private readonly DataContextUsers _dataContextUsers;
        private readonly IConverterHelper _converterHelper;


        public MessageController(IMessageRepository messageRepository, DataContextUsers contextUsers, IConverterHelper converterHelper)
        {
            _messageRepository = messageRepository;
            _dataContextUsers = contextUsers;
            _converterHelper = converterHelper; 
        }


        // GET: MessageController
        [HttpGet("GetAllMessages")]
        public ActionResult<IEnumerable<MessageDto>> GetAllMessages()
        {
            var messages = _messageRepository.GetAll(_dataContextUsers);

            var messagesDto = messages.Select(m => _converterHelper.ToMessageDto(m)).ToList();

            return messagesDto;
        }

        // GET: MessageController/MessageDetails/5
        [HttpGet("MessageDetails")]
        public ActionResult MessageDetails(int id)
        {
            return View();

        }


        // POST: MessageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMessageStatus(int id, IFormCollection collection)
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
