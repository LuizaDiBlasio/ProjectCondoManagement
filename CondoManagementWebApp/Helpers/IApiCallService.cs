using System.Net.Http;
using System.Text;

namespace CondoManagementWebApp.Helpers
{
    public interface IApiCallService
    {
        Task<IEnumerable<T>> GetAllAsync(string requestUri);

        public void AddAuthorizationHeader();

        public Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data);

        public Task<T> GetAsync<T>(string requestUri);

        public Task<TResponse> GetByEmailAsync<TRequest, TResponse>(string requestUri, TRequest data); //TRequest será sempre string nesse caso

        public Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}
