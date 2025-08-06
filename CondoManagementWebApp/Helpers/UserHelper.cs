using ClassLibrary.DtoModels;

namespace CondoManagementWebApp.Helpers
{
    public class UserHelper : ApiCallService<RegisterUserDto>, IUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public UserHelper(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
             : base(httpClient, httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }
    }
}
