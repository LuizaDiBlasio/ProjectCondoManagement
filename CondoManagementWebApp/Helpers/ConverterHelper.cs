using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;

namespace CondoManagementWebApp.Helpers
{
    public class ConverterHelper
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

        public RegisterUserDto ToRegisterDto(RegisterUserViewModel model, string imageUrl)
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
                ImageUrl = imageUrl
            };
            return registerDto;
        }
    }
}
