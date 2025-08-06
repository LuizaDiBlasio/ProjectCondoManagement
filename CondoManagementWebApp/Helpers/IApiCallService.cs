namespace CondoManagementWebApp.Helpers
{
    public interface IApiCallService<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAllAsync(string requestUri);

        Task<T> GetByIdAsync(string requestUri);


        Task<bool> CreateAsync(string requestUri, T obj);

        Task<bool> EditAsync(string requestUri,T obj);

        Task<bool> DeleteAsync(string requestUri);
    }
}
