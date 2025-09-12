using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondoMemberRepository : IGenericRepository<CondoMember, DataContextCondos>
    {
        public Task<Response<object>> LinkImages(IEnumerable<CondoMember> condoMembers);

        public Task<CondoMember> GetCondoMemberByEmailAsync(string email);
        public Task<CondoMember?> GetByIdWithIncludeAsync(int id, DataContextCondos context);
        public Task<bool> AssociateFinancialAccountAsync(string? email, int? financialAccountId);
    }
}
