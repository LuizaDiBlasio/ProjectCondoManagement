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

        public CondoMemberDto ToCondoMemberDto(CondoMember condoMember)
        {
            var condoMemberDto = new CondoMemberDto
            {
                Id = condoMember.Id,
                FullName = condoMember.FullName,
                Email = condoMember.Email,
                Address = condoMember.Address,
                BirthDate = condoMember.BirthDate,
                PhoneNumber = condoMember.PhoneNumber,
                ImageUrl = condoMember.ImageUrl
            };

            return condoMemberDto;
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

        public UserDto ToUserDto(User user)
        {
            var userDto = new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImageUrl = user.ImageUrl,
                Email = user.Email
            };
            return userDto; 
        }

        public Condominium ToCondominium(CondominiumDto condominiumDto)
        {
            var condominium = new Condominium
            {
                Id = condominiumDto.Id,
                Address = condominiumDto.Address,
                CompanyId = condominiumDto.CompanyId.Value,
                CondoName = condominiumDto.CondoName,
                ManagerUserId = condominiumDto.ManagerUserId,

            };

            return condominium;
        }

        public CondominiumDto ToCondominiumDto(Condominium condominium)
        {
            var condominiumDto = new CondominiumDto
            {
                Id = condominium.Id,
                Address = condominium.Address,
                CompanyId = condominium.CompanyId,
                CondoName = condominium.CondoName,
                ManagerUserId = condominium.ManagerUserId

            };

            return condominiumDto;
        }

        public CompanyDto ToCompanyDto(Company company)
        {
            var companyDto = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                PhoneNumber = company.PhoneNumber,
                Email = company.Email,
                Addres = company.Addres,
                TaxIdDocument = company.TaxIdDocument

            };

            return companyDto;
        }
    }
}
