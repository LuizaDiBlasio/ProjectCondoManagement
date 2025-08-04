using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IConverterHelper
    {
        CondoMember ToCondoMember(CondoMemberDto condoMemberDto);
        public CondoMemberDto ToCondoMemberDto(User user);
    }
}
