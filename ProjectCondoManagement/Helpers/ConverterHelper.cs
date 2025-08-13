using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public CondoMember ToCondoMember(CondoMemberDto condoMemberDto)
        {
            var condoMember = new CondoMember
            {
                Id = condoMemberDto.Id,
                FullName = condoMemberDto.FullName,
                Email = condoMemberDto.Email,
                Address = condoMemberDto.Address,
                BirthDate = condoMemberDto.BirthDate,
                PhoneNumber = condoMemberDto.PhoneNumber,
                ImageUrl = condoMemberDto.ImageUrl,
            };

            return condoMember;
        }

        public CondoMemberDto ToCondoMemberDto(User user)
        {
            var condoMemberDto = new CondoMemberDto
            {               
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
            };
            return condoMemberDto;
        }

        public Condominium ToCondominium(CondominiumDto condominiumDto)
        {
            var condominium = new Condominium
            {
                Id = condominiumDto.Id,
                Address = condominiumDto.Address,
                CompanyId = condominiumDto.CompanyId,
                CondoName = condominiumDto.CondoName,
                ManagerUserId = condominiumDto.ManagerUserId,

            };

            return condominium;
        }
    }
}
