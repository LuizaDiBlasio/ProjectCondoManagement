
using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace CondoManagementWebApp.Helpers
{
    public class ApiCallService<T> : IApiCallService<T> where T : class, new()
    {
        private readonly HttpClient _httpClient;    
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiCallService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<T>> GetAllAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var tList = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);

                return tList ?? Enumerable.Empty<T>() ;
            }

            return Enumerable.Empty<T>();
        }


        public async Task<T> GetByIdAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var obj = JsonConvert.DeserializeObject<T>(jsonString);

                return obj ?? new T();
            }

            return new T();
        }




        public async Task<bool> CreateAsync(string requestUri, T obj)
        {
            AddAuthorizationHeader();

            var jsonContent = new StringContent(
               System.Text.Json.JsonSerializer.Serialize(obj),
               Encoding.UTF8,
               "application/json"
           );

            var response = await _httpClient.PostAsync(requestUri, jsonContent);


            return response.IsSuccessStatusCode;
        }



        public async Task<bool> EditAsync(string requestUri, T obj)
        {
            AddAuthorizationHeader();
            var jsonContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(obj),
                Encoding.UTF8,
                "application/json"
            );
            

            var response = await _httpClient.PostAsync(requestUri, jsonContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string requestUri)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(requestUri);
            return response.IsSuccessStatusCode;
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

