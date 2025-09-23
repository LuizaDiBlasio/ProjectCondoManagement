using ClassLibrary;
using ClassLibrary.DtoModels;
using ClassLibrary.DtoModelsMobile;

namespace MobileCondoManagement.Services.Interfaces
{
    public interface IApiService
    {
        Task<LoginResponseDto> RequestLoginAsync(string email, string password);

        Task<Response<object>> RequestForgotPasswordAsync(string email);

        Task<Response<object>> ResetPasswordAsync(ResetPasswordDto resetData);

        Task<T> GetAsync<T>(string requestUri);

        Task<TResponse> GetByQueryAsync<TResponse>(string requestUri, string query);
    }
}

