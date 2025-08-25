using ClassLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Users
{
    public interface IMessageRepository : IGenericRepository<Message, DataContextUsers>
    {
        public List<SelectListItem> GetMessageStatusList();
    }
}