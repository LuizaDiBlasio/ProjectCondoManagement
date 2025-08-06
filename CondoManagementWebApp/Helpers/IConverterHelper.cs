using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;

namespace CondoManagementWebApp.Helpers
{
    public interface IConverterHelper
    {
        public LoginDto ToLoginDto(LoginViewModel model);

        public RegisterUserDto ToRegisterDto(RegisterUserViewModel model);

        public RegisterUserDto ToRegisterDto(CondoMemberDto condoMember);
    }
}
