using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Net.Http;

namespace CondoManagementWebApp.Helpers
{
    public class CondoMemberHelper : ApiCallService<CondoMemberDto>, ICondoMemberHelper 
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CondoMemberHelper(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
             : base(httpClient, httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
             _httpClient.BaseAddress = new Uri("https://localhost:7001/");
        }

        public async Task<IEnumerable<CondoMemberDto>> GetCondoMembersAsync()
        {
            using (var client = new HttpClient())
            {
                var apiUrl = "https://localhost:7001/api/CondoMembers";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var condoMembers = JsonConvert.DeserializeObject<IEnumerable<CondoMemberDto>>(jsonString);
                    if (condoMembers == null)
                    {
                        return Enumerable.Empty<CondoMemberDto>();
                    }
                    return condoMembers;
                }


                return Enumerable.Empty<CondoMemberDto>();
            }

      
        }


        public async Task<CondoMemberDto> GetCondoMemberAsync(int id)
        {
            using (var client = new HttpClient())
            {
                var apiUrl = $"https://localhost:7001/api/CondoMembers/{id}";

                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var condoMember = JsonConvert.DeserializeObject<CondoMemberDto>(jsonString);

                    return condoMember ?? new CondoMemberDto();
                }

                return new CondoMemberDto();
            }

        }

        public async Task<bool> CreateCondoMemberAsync(CondoMemberDto condoMemberDto)
        {
            using (var client = new HttpClient())
            {
                var apiUrl = "https://localhost:7001/api/CondoMembers";

                var jsonContent = JsonConvert.SerializeObject(condoMemberDto);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, httpContent);
            

                return response.IsSuccessStatusCode;
            }
        }


        public async Task<bool> EditCondoMemberAsync(CondoMemberDto condoMemberDto)
        {
            using (var client = new HttpClient())
            {
                var apiUrl = $"https://localhost:7001/api/CondoMembers/Edit/{condoMemberDto.Id}";


                var jsonContent = JsonConvert.SerializeObject(condoMemberDto);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, httpContent);


                return response.IsSuccessStatusCode;
            }
        }


        public async Task<bool> DeleteCondoMemberAsync(int id)
        {
            using (var client = new HttpClient())
            {
                var apiUrl = $"https://localhost:7001/api/CondoMembers/{id}";

                var response = await client.DeleteAsync(apiUrl);

                return response.IsSuccessStatusCode;
            }
        }


    }
}
