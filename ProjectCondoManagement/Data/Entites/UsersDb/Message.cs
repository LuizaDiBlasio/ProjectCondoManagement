using ClassLibrary;
using ProjectCondoManagement.Data.Entites.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class Message : IEntity
    {
        public int Id { get; set; }

        public DateTime PostingDate { get; set; }

        public string MessageTitle { get; set; }

        public string MessageContent { get; set; }

        public string SenderEmail { get; set; }

        public string ReceiverEmail { get; set; }  

        public MessageStatus Status { get; set; }
    }
}
