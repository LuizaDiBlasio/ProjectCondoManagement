using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace CondoManagementWebApp.Helpers
{
    public class ApiCallService : IApiCallService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiCallService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor; //possibilita aceder ao httpContext fora do controller 
            _httpClient.BaseAddress = new Uri("https://localhost:7001/");  // adiciona base address ao httpClient //TODO : Mudar depois que publicar
        }

        public void AddAuthorizationHeader()
        {
            var jwtToken = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwtToken))
            {
            response.EnsureSuccessStatusCode(); // Lança exceção se o status não for sucesso
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tList = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tList = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tList = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            response.EnsureSuccessStatusCode(); // Lança exceção se o status não for sucesso
            if (response.IsSuccessStatusCode)
            {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(requestUri);
            response.EnsureSuccessStatusCode(); // Lança exceção para status codes de erro
            return response;

            return await response.Content.ReadFromJsonAsync<T>();
        public async Task<TResponse> GetByEmailAsync<TRequest, TResponse>(string requestUri, TRequest data)
        {
            AddAuthorizationHeader();
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync(requestUri, jsonContent);
            response.EnsureSuccessStatusCode();
                System.Text.Json.JsonSerializer.Serialize(obj),
            return await response.Content.ReadFromJsonAsync<TResponse>();
            var response = await _httpClient.PostAsync(requestUri, jsonContent);

}
        }
                System.Text.Json.JsonSerializer.Serialize(obj),
        public async Task<bool> DeleteAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(requestUri);
            return response.IsSuccessStatusCode;
            var response = await _httpClient.PostAsync(requestUri, jsonContent);

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

        }
                System.Text.Json.JsonSerializer.Serialize(obj),
        public async Task<bool> DeleteAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(requestUri);
            return response.IsSuccessStatusCode;
            var response = await _httpClient.PostAsync(requestUri, jsonContent);

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

        }
            response.EnsureSuccessStatusCode();
        public async Task<bool> DeleteAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(requestUri);
            return response.IsSuccessStatusCode;
            return await response.Content.ReadFromJsonAsync<TResponse>();
        {
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

//        return RedirectToAction("Login", "Account");
//    }
//}

