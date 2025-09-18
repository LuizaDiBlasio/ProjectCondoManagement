using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondominiumRepository : IGenericRepository<Condominium, DataContextCondos>
    {
        Task<Condominium> GetCondoManagerCondominium(string id);

        Task<Response<object>> LinkManager(List<CondominiumDto> condominiums);

        Task<List<Condominium>> GetCompanyCondominiums(List<int> condominiumsIds);

        Task<List<CondoMember>> GetCondoCondomembers(int condoId);

        Task<Response<object>> LinkFinancialAccountWithTransactions(List<CondominiumDto> condominiums);

        Task<Response<object>> LinkFinancialAccount(List<CondominiumDto> condominiums);
        Task<IEnumerable<CondominiumDto>?> GetCondominiumsByCompanyIdAsync(int id);
        Task<Condominium> GetCondominiumByFinancialAccountIdAsync(int id);
    }
}
