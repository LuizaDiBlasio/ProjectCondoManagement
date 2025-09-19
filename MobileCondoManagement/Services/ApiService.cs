using ClassLibrary;
using ClassLibrary.DtoModelsMobile;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MobileCondoManagement.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Json; 
using System.Text;
using System.Text.Json; 

namespace MobileCondoManagement.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://10.0.2.2:7001/");
        }


        public async Task<LoginResponseDto> RequestLoginAsync(string email, string password)
        {
            var loginData = new { Email = email, Password = password };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(loginData), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Account/LoginMobile", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(json);
                return loginResponse;
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<LoginResponseDto>(json);
                return errorResponse ?? new LoginResponseDto { IsSuccess = false, Message = "Login failed. Check your credentials." };
            }
        }

        public async Task<Response<object>> RequestForgotPasswordAsync(string email)
        {
            var forgotPasswordData = new { Email = email};
            var jsonContent = new StringContent(JsonConvert.SerializeObject(forgotPasswordData), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Account/GenerateForgotPasswordTokenAndEmail", jsonContent);

            var json = await response.Content.ReadAsStringAsync();

            // Deserializa a resposta em um objeto dinâmico para pegar a propriedade 'Message' 
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            if (response.IsSuccessStatusCode)
            {
                return new Response<object> { IsSuccess = true, Message = (string)result.Message };
            }
            else
            {
                return new Response<object> { IsSuccess = false, Message = (string)result.Message };
            }
        }

        #region ApiGenericCalls

        public async Task AddAuthorizationHeader()
        {
            var jwtToken = await SecureStorage.GetAsync("auth_token");
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
                System.Text.Json.JsonSerializer.Serialize(data),
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

        #endregion

    }
}
