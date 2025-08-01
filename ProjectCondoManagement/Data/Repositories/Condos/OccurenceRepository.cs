using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class OccurrenceRepository : GenericRepository<Occurrence, DataContextCondos>, IOccurenceRepository
    {
    }
}
