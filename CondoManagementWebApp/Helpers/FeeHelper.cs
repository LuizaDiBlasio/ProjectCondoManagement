using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace CondoManagementWebApp.Helpers
{
    public class FeeHelper : IFeeHelper
    {
        private readonly IApiCallService _apiCallService;
  
        public FeeHelper(IApiCallService apiCallService)
        {
            _apiCallService = apiCallService;
        }

        public async Task<IEnumerable<FeeDto>> GetFeesAsync()
        {
            var fees = await _apiCallService.GetAsync<IEnumerable<FeeDto>>("api/Fees/GetFees");

            return fees;
        }
    }
}
