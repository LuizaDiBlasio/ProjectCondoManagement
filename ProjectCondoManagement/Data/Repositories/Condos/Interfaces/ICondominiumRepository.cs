using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondominiumRepository : IGenericRepository<Condominium, DataContextCondos>
    {
        Task<Response<object>> LinkManager(List<CondominiumDto> condominiums);
    }
}
