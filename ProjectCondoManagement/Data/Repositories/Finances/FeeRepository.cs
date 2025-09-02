using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Helpers;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class FeeRepository : GenericRepository<Fee,DataContextFinances>, IFeeRepository
    {
        private readonly DataContextFinances _contextFinances;
        private readonly IConverterHelper _converterHelper;

        public FeeRepository(DataContextFinances contextFinances,IConverterHelper converterHelper)
        {
            _contextFinances = contextFinances;
            _converterHelper = converterHelper;
        }

        public async Task<IEnumerable<Fee>> GetAllFees()
        {
            var fees = await _contextFinances.Fees.ToListAsync();

            return fees;
        }

        public async Task UpdateFees(IEnumerable<FeeDto> updatedFees)
        {
            foreach (var feeDto in updatedFees)
            {
                var fee = _converterHelper.ToFee(feeDto);
                await UpdateAsync(fee, _contextFinances);
            }
        }
    }
}
