using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface ICondoMemberRepository : IGenericRepository<CondoMember, DataContextCondos>
    {
        Task<Response> LinkImages(IEnumerable<CondoMember> condoMembers); // Link images to condo members
    }
}
