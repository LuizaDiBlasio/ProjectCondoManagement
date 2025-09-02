using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace CondoManagementWebApp.Helpers
{
    public interface IFeeHelper
    {
        Task<IEnumerable<FeeDto>> GetFeesAsync();
    }
}
