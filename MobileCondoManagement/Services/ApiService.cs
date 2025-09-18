using ClassLibrary.DtoModelsMobile;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MobileCondoManagement.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://10.0.2.2:5081/");
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
    }
}
