using ClassLibrary;
using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface IFeeRepository : IGenericRepository<Fee,DataContextFinances>
    {
        Task<IEnumerable<Fee>> GetAllFees();

        Task UpdateFees(IEnumerable<FeeDto> updatedFees);

    }
}
