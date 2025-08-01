using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.CondosDb;
using ProjectCondoManagementAPI.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagementAPI.Data.Repositories.Condos
{
    public class OccurrenceRepository : GenericRepository<Occurrence, DataContextCondos>, IOccurenceRepository
    {
    }
}
