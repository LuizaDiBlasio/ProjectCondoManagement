using ClassLibrary;
using ProjectCondoManagementAPI.Data.Entites.CondosDb;

namespace ProjectCondoManagementAPI.Data.Repositories.Condos.Interfaces
{
    public interface ICondominiumRepository : IGenericRepository<Condominium, DataContextCondos>
    {
    }
}
