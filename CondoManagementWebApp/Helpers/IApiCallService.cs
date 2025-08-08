namespace CondoManagementWebApp.Helpers
{
    public interface IApiCallService
    {
        //Task<IEnumerable<T>> GetAllAsync(string requestUri);

        //Task<T> GetByIdAsync(string requestUri);


        //Task<bool> CreateAsync(string requestUri, T obj);

        //Task<bool> EditAsync(string requestUri,T obj);

        //Task<bool> DeleteAsync(string requestUri);


        public void AddAuthorizationHeader();

        public Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data);

        public Task<T> GetAsync<T>(string requestUri);

        public Task<HttpResponseMessage> DeleteAsync(string requestUri);

    }
}
