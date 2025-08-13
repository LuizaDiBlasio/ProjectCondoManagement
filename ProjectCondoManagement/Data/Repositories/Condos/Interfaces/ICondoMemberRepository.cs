using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondoMemberRepository : IGenericRepository<CondoMember, DataContextCondos>
    {
        public Task<Response> LinkImages(IEnumerable<CondoMember> condoMembers);
    }
}
