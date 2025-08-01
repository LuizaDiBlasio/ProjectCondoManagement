using ClassLibrary;

namespace ProjectCondoManagementAPI.Helpers
{
    public interface IMailHelper
    {
      Response SendEmail(string email, string subject, string body);
    }
}
