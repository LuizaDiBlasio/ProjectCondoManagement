using System.Text;
using System.Text.Json;

namespace CondoManagementWebApp.Helpers
{
    public class UserApiCallService : IUserApiCallService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserApiCallService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor; //possibilita aceder ao httpContext fora do controller 
            _httpClient.BaseAddress = new Uri("https://localhost:7001/");  // adiciona base address ao httpClient //TODO : Mudar depois que publicar
        }

        private void AddAuthorizationHeader()
        {
            var jwtToken = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }

        public async Task<T> GetAsync<T>(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode(); // Lança exceção se o status não for 2xx

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest data)
        {
            AddAuthorizationHeader();
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync(requestUri, jsonContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest data)
        {
            AddAuthorizationHeader();
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PutAsync(requestUri, jsonContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(requestUri);
            response.EnsureSuccessStatusCode(); // Lança exceção para status codes de erro
            return response;
        }
    }
}

//exemplo de uso para colegas:
//public class CondosController : Controller

//private readonly IApiService _apiService;

//public CondosController(IApiService apiService)
//{
//    _apiService = apiService;
//}

//[Authorize]
//public async Task<IActionResult> Index()
//{
//    try
//    {
//        // Usando o método genérico para buscar uma lista de condomínios
//        var condos = await _apiService.GetAsync<List<CondoModel>>("api/condos");
//        return View(condos);
//    }
//    catch (HttpRequestException ex)
//    {
//        // Lógica para lidar com erros da API, como 401 Unauthorized
//        // Redirecionar para o login pode ser uma boa ideia
//        return RedirectToAction("Login", "Account");
//    }
//}

