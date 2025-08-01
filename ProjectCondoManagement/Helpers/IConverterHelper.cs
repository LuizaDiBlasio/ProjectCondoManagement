using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IConverterHelper
    {
        public CondoMemberDto ToCondoMemberDto(User user);
    }
}
