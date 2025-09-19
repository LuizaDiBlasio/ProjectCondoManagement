
using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CondoManagementWebApp.Helpers
{
    public class ApiCallService : IApiCallService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; 

        public ApiCallService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration )
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor; //possibilita aceder ao httpContext fora do controller 
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri($"{_configuration["ApiSettings:BaseUrl"]}");  // adiciona base address ao httpClient 
            _configuration = configuration;
        }

        public void AddAuthorizationHeader()
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

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Erro {response.StatusCode}. " +
                    $"Detalhes: {await response.Content.ReadAsStringAsync()}");
            }

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

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log ou retorne o conteúdo do erro
                throw new Exception($"API call failed with status code {response.StatusCode}. Content: {errorContent}");
            }

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


        public async Task<TResponse> GetByQueryAsync<TResponse>(string requestUri, string query)
        {
            AddAuthorizationHeader();

            // Serializa o objeto
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(query),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(requestUri, jsonContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }


    }
}