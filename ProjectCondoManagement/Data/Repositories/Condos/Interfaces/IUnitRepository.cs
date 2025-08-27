using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface IUnitRepository : IGenericRepository<Unit, DataContextCondos>
    {
        Task<Unit?> GetByIdWithIncludeAsync(int id, DataContextCondos context);
    }
}
