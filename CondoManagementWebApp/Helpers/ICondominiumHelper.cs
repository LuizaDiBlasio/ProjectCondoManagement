using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace CondoManagementWebApp.Helpers
{
    public interface ICondominiumHelper
    {
        Task<IEnumerable<CondominiumDto>> GetCondominiumsAsync(string email);
    }
}
