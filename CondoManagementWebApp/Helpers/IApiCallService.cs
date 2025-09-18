using System.Net.Http;
using System.Text;

namespace CondoManagementWebApp.Helpers
{
    public interface IApiCallService
    {
        void AddAuthorizationHeader();

        Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data);

        Task<T> GetAsync<T>(string requestUri);

        Task<TResponse> GetByQueryAsync<TResponse>(string requestUri, string query);

        Task<HttpResponseMessage> DeleteAsync(string requestUri);

    }
}
