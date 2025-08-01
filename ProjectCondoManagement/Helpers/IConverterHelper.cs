using ClassLibrary.DtoModels;
using ProjectCondoManagementAPI.Data.Entites.UsersDb;

namespace ProjectCondoManagementAPI.Helpers
{
    public interface IConverterHelper
    {
        public CondoMemberDto ToCondoMemberDto(User user);
    }
}
