using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Helpers
{
    

    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly ICondoMemberRepository _condoMemberRepository;

        public ConverterHelper(IUserHelper userHelper, ICondoMemberRepository condoMemberRepository)
        {
            _userHelper = userHelper;
            _condoMemberRepository = condoMemberRepository;

        }

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
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImageUrl = user.ImageUrl,
                IsActive = user.IsActive,
                Email = user.Email,
                CompanyId = user.CompanyId, 
                FinancialAccountId = user.FinancialAccountId,
            };
            return userDto; 
        }

        public async Task<User> ToEditedUser(EditUserDetailsDto editUserDetailsDto)
        {
           var user = await _userHelper.GetUserByEmailAsync(editUserDetailsDto.Email);

            if (user == null)
            {
                return null;
            }

            user.FullName = editUserDetailsDto.FullName; 
            user.BirthDate = editUserDetailsDto.BirthDate;
            user.PhoneNumber = editUserDetailsDto.PhoneNumber;
            user.Address = editUserDetailsDto.Address;
            user.ImageUrl = editUserDetailsDto.ImageUrl;
            user.Email = editUserDetailsDto.Email;  
            user.IsActive = editUserDetailsDto.IsActive;
            user.CompanyId = editUserDetailsDto.CompanyId;  
            user.FinancialAccountId = editUserDetailsDto.FinancialAccountId;    

            return user;
        }

        public async Task<User> ToEditedProfile(UserDto userDto)
        {
            var user = await _userHelper.GetUserByEmailAsync(userDto.Email);

            if (user == null)
            {
                return null;
            }

            user.FullName = userDto.FullName;
            user.BirthDate = userDto.BirthDate;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Address = userDto.Address;
            user.ImageUrl = userDto.ImageUrl;
            user.Email = userDto.Email;

            return user;
        }

        public async Task<CondoMember> FromUserToCondoMember(User user)
        {
            var condoMember = await _condoMemberRepository.GetCondoMemberByEmailAsync(user.Email);

            if (condoMember == null)
            {
                return null;
            }

            condoMember.FullName = user.FullName;
            condoMember.PhoneNumber = user.PhoneNumber;
            condoMember.Address = user.Address;
            condoMember.ImageUrl = user.ImageUrl;   
            condoMember.BirthDate = user.BirthDate;

            return condoMember; 
        }
    }
}
