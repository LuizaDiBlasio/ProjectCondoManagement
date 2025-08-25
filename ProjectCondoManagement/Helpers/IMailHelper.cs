using ClassLibrary;

namespace ProjectCondoManagement.Helpers
{
    public interface IMailHelper
    {
      Response<object> SendEmail(string email, string subject, string body);

      
    }
}
