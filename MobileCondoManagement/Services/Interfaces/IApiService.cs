using ClassLibrary;
using ClassLibrary.DtoModelsMobile;

namespace MobileCondoManagement.Services.Interfaces
{
    public interface IApiService
    {
        Task<LoginResponseDto> RequestLoginAsync(string email, string password);

        Task<Response<object>> RequestForgotPasswordAsync(string email);
    }
}

