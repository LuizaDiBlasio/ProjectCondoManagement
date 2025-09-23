using ClassLibrary;
using ClassLibrary.DtoModels;
using ClassLibrary.DtoModelsMobile;

namespace MobileCondoManagement.Services.Interfaces
{
    public interface IApiService
    {
        Task<LoginResponseDto> RequestLoginAsync(string email, string password);

        Task<Response<object>> RequestForgotPasswordAsync(string email);

        Task<T> GetAsync<T>(string requestUri);

        Task<Response<object>> ResetPasswordAsync(ResetPasswordDto resetData);

        Task<TResponse> GetByQueryAsync<TResponse>(string requestUri, string query);
    }
}

