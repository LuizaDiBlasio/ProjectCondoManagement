using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondominiumRepository : IGenericRepository<Condominium, DataContextCondos>
    {
        Task<Condominium> GetCondoManagerCondominium(string id);

        Task<Response<object>> LinkManager(List<CondominiumDto> condominiums);

        Task<List<Condominium>> GetCompanyCondominiums(List<int> condominiumsIds);

        Task UpdateCondominiumsCompanyId(Company company);
    }
}
