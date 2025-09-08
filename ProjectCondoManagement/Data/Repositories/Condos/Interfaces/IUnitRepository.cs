using ClassLibrary;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface IUnitRepository : IGenericRepository<Unit, DataContextCondos>
    {
        Task<Unit?> GetByIdWithIncludeAsync(int id, DataContextCondos context);

        Task<List<SelectListItem>> GetCondoUnitsList(int id);
    }
}
