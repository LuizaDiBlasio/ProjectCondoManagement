namespace CondoManagementWebApp.Helpers
{
    public interface IApiCallService
    {
        Task<T> GetAsync<T>(string requestUri);

        Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data);

        Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest data);

        Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}
