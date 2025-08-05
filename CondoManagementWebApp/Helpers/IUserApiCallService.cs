namespace CondoManagementWebApp.Helpers
{
    public interface IUserApiCallService
    {
        Task<T> GetAsync<T>(string requestUri);

        Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data);

        //Não sei se vamos usar
        Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest data);

        Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}
