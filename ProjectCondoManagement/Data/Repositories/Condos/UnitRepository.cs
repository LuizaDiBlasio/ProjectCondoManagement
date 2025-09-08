using ClassLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class UnitRepository : GenericRepository<Unit, DataContextCondos>, IUnitRepository
    {
        private readonly DataContextCondos _dataContextCondos;

        public UnitRepository(DataContextCondos dataContextCondos)
        {
            _dataContextCondos = dataContextCondos;
        }

        public async Task<Unit?> GetByIdWithIncludeAsync(int id, DataContextCondos context)
        {
            return await context.Units .Include(u => u.Condominium).Include(u => u.CondoMembers).FirstOrDefaultAsync(u => u.Id == id);

        }

        public async Task<List<SelectListItem>> GetCondoUnitsList(int id)
        {
            var unitsList = await _dataContextCondos.Units.Where(u => u.CondominiumId == id).ToListAsync();

            return unitsList.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(), 
                Text = $"Door {u.Door.ToString()} - Floor {u.Floor.ToString()}"
            }).ToList();
        }
    }
}
