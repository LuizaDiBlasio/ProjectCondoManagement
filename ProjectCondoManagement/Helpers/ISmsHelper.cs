using ClassLibrary;

namespace ProjectCondoManagement.Helpers
{
    public interface ISmsHelper
    {
        Task<Response> SendSmsAsync(string phoneNumber, string message);
    }
}
