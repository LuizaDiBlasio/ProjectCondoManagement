using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;

namespace ProjectCondoManagement.Data.Repositories.Condos.Interfaces
{
    public interface IOccurenceRepository : IGenericRepository<Occurrence, DataContextCondos>
    {
        Task<Occurrence> GetOccurrenceWithUnits(int id);

        Task<List<Unit>> GetSelectedUnits(List<int> selectedUnits);
    }
}
