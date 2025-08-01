using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface IVoteRepository : IGenericRepository<Vote, DataContextCondos>
    {
    }
}
