using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;

namespace CondoManagementWebApp.Helpers
{
    public interface IConverterHelper
    {
        public LoginDto ToLoginDto(LoginViewModel model);

        public RegisterUserDto ToRegisterDto(RegisterUserViewModel model);

        public ResetPasswordDto ToResetPasswordDto(ResetPasswordViewModel model); //userId token

        public RegisterUserDto ToRegisterDto(CondoMemberDto condoMember);

    }
}
