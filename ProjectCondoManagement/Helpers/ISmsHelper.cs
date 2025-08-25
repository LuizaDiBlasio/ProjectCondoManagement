using ClassLibrary;

namespace ProjectCondoManagement.Helpers
{
    public interface ISmsHelper
    {
        Task<Response<object>> SendSmsAsync(string phoneNumber, string message);
    }
}
