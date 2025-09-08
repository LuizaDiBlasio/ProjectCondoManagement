using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class OccurrenceRepository : GenericRepository<Occurrence, DataContextCondos>, IOccurenceRepository
    {
        private readonly DataContextCondos _dataContextCondos;

        public OccurrenceRepository (DataContextCondos dataContextCondos)
        {
            _dataContextCondos = dataContextCondos;
        }


        public async Task<Occurrence> GetOccurrenceWithUnits(int id)
        {
            return await _dataContextCondos.Occurences
                                    .Where(o => o.Id == id)
                                    .Include(o => o.Units)
                                    .ThenInclude(u => u.Condominium)
                                    .FirstOrDefaultAsync();
        }

        public async Task<List<Unit>> GetSelectedUnits(List<int> selectedUnits)
        {
            return await _dataContextCondos.Units
                .Where(u => selectedUnits
                .Contains(u.Id))
                .Include(u => u.Condominium)
                .ToListAsync();
        }
    }
}
