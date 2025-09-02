
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Helpers;
using ClassLibrary.DtoModels;
using Azure;

namespace CondoManagementWebApp.Helpers
{
    public class CondominiumHelper : ICondominiumHelper
    {
        private readonly IApiCallService _apiCallService;

        public CondominiumHelper(IApiCallService apiCallService)
        {
            _apiCallService = apiCallService;
        }

        public async Task<IEnumerable<CondominiumDto>> GetCondominiumsAsync(string email)
        {
            var condominiums = await _apiCallService.GetAsync<IEnumerable<CondominiumDto>>("api/Condominiums/GetCondominiums");

            var companyAdmin = await _apiCallService.GetByQueryAsync<UserDto>("api/Account/GetUser",$"{email}");

            return condominiums.Where(c => c.ManagerUserId == companyAdmin.Id);
        }
    }
}

