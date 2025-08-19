using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IConverterHelper
    {
        CompanyDto? ToCompanyDto(Company company);

        public CondoMember ToCondoMember(CondoMemberDto condoMemberDto);

       public CondoMemberDto ToCondoMemberDto(CondoMember condoMember);

       public CondoMemberDto ToCondoMemberDto(User user);

       public Condominium ToCondominium(CondominiumDto condominiumDto);

       public CondominiumDto ToCondominiumDto(Condominium condominium);

       public UserDto ToUserDto(User user);    
    }
}
