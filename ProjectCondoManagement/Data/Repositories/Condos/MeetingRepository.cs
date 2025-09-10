using ClassLibrary;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class MeetingRepository : GenericRepository<Meeting, DataContextCondos>, IMeetingRepository
    {
        private readonly DataContextCondos _dataContextCondos;

        public MeetingRepository(DataContextCondos dataContextCondos)
        {
            _dataContextCondos = dataContextCondos;
        }

        public async Task<Meeting> GetMeetingWithCondomembersAndOccurrences (int id)
        {
            return await _dataContextCondos.Meeting
                            .Include(m => m.Occurences)
                            .Include(m => m.CondoMembers)
                            .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
