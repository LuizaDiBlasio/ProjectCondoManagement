using ClassLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.Enums;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public class MessageRepository : GenericRepository<Message, DataContextUsers>, IMessageRepository
    {
        public List<SelectListItem> GetMessageStatusList()
        {
                    return Enum.GetValues(typeof(MessageStatus)) //indicar o tipo do enum
                            .Cast<MessageStatus>() // converter ints do enum para lista IEnumerable<MessageStatus>
                             .Select(status => new SelectListItem //converter para lista SelectListItem 
                             {
                                 Value = ((int)status).ToString(),
                                 Text = status.ToString()
                             }).ToList();
  
        }



        public async Task<IEnumerable<Message>> GetReceivedMessagesAsync(string receiverEmail, DataContextUsers context)
        {
            return await context.Messages
                .Where(m => m.ReceiverEmail == receiverEmail && !m.DeletedByReceiver)
                .ToListAsync();
        }



    }
}
