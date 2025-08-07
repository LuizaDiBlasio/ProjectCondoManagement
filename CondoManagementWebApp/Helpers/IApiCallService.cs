using System.Net.Http;
using System.Text;

namespace CondoManagementWebApp.Helpers
{
    public interface IApiCallService
    {
        public void AddAuthorizationHeader();

        public Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data);

        public  Task<T> GetAsync<T>(string requestUri);

        public Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}
