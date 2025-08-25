using ClassLibrary;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class UnitRepository : GenericRepository<Unit, DataContextCondos>, IUnitRepository
    {
        public async Task<Unit?> GetByIdWithIncludeAsync(int id, DataContextCondos context)
        {
            return await context.Units .Include(u => u.Condominium).Include(u => u.CondoMembers).FirstOrDefaultAsync(u => u.Id == id);

        }
    }
}
