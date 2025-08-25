using ClassLibrary;
using ClassLibrary.DtoModels;

namespace ProjectCondoManagement.Helpers
{
    public interface ICondominiumHelper
    {
        public Task<Response<object>> LinkCompanyToCondominiumAsync(CondominiumDto condominium);
    }
}
