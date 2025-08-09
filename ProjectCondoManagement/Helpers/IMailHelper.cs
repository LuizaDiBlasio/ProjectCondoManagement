using ClassLibrary;

namespace ProjectCondoManagement.Helpers
{
    public interface IMailHelper
    {
      Response SendEmail(string email, string subject, string body);

      
    }
}
