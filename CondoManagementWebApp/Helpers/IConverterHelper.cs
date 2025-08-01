using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;

namespace CondoManagementWebApp.Helpers
{
    public interface IConverterHelper
    {
        public LoginDto ToLoginDto(LoginViewModel model);

        public RegisterDto ToRegisterDto(RegisterUserViewModel model, Guid imageId);
    }
}
