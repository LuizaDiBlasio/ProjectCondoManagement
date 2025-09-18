using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondoMemberRepository : IGenericRepository<CondoMember, DataContextCondos>
    {
        Task<Response<object>> LinkImages(IEnumerable<CondoMember> condoMembers);

        Task<CondoMember> GetCondoMemberByEmailAsync(string email, bool includeUnitsAndCondominums = true);

        Task<CondoMember?> GetByIdWithIncludeAsync(int id, DataContextCondos context);

        Task<bool> AssociateFinancialAccountAsync(string? email, int? financialAccountId);

        Task<List<CondoMember>> GetCondoMembersByEmailsAsync(List<string> emails);
        Task<bool> ExistByEmailAsync(string email);
    }
}
