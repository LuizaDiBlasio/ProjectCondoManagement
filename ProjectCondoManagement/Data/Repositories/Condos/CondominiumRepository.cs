using ClassLibrary;
using ClassLibrary.DtoModels;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Threading.Tasks;

namespace ProjectCondoManagement.Data.Repositories.Condos
{
    public class CondominiumRepository : GenericRepository<Condominium, DataContextCondos>, ICondominiumRepository
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public CondominiumRepository(IUserHelper userHelper, IConverterHelper converterHelper) 
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

  

        public async Task<Response<object>> LinkManager(List<CondominiumDto> condominiums)
        {
            try
            {
                IEnumerable<User> managers = await  _userHelper.GetUsersByRoleAsync("CondoManager");
                if (managers == null || !managers.Any())
                {
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = "No managers found"
                    };
                }


                foreach (CondominiumDto condominium in condominiums)
                {
                    var manager = managers.FirstOrDefault(m => m.Id == condominium.ManagerUserId);
                    if (manager != null)
                    {
                        var managerDto = _converterHelper.ToUserDto(manager);

                        condominium.ManagerUser = managerDto;
                    }
                }

                return new Response<object>
                {
                    IsSuccess = true,
                    Message = "Images linked successfully"
                };
            }
            catch (Exception ex)
            {

                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"Error linking images: {ex.Message}"
                };
            }


        }

    }
}
