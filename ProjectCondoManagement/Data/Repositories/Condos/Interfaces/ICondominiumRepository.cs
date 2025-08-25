using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondominiumRepository : IGenericRepository<Condominium, DataContextCondos>
    {
        Task<Condominium> GetCondoManagerCondominium(string id);

        Task<Response> LinkManager(List<CondominiumDto> condominiums);

        Task<List<Condominium>> GetCompanyCondominiums(List<int> condominiumsIds);
    }
}
