using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;

namespace CondoManagementWebApp.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public LoginDto ToLoginDto(LoginViewModel model)
        {
            var loginDto = new LoginDto
            {
                Username = model.Username,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            return loginDto;
        }

        public RegisterUserDto ToRegisterDto(RegisterUserViewModel model)
        {
            var registerDto = new RegisterUserDto
            {
                FullName = model.FullName,
                Email = model.Email,
                Address = model.Address,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber,
                SelectedRole = model.SelectedRole,
                CompanyId = model.SelectedCompanyId,
                ImageUrl = model.ImageUrl
            };
            return registerDto;
        }

        public RegisterUserDto ToRegisterDto(CondoMemberDto condoMember)
        {
            var registerDto = new RegisterUserDto
            {
                FullName = condoMember.FullName,
                Email = condoMember.Email,
                Address = condoMember.Address,
                BirthDate = condoMember.BirthDate,
                PhoneNumber = condoMember.PhoneNumber,
                ImageUrl = condoMember.ImageUrl
            };
            return registerDto;
        }

    }
}
