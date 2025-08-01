using ClassLibrary.DtoModels;
using ProjectCondoManagementAPI.Data.Entites.UsersDb;

namespace ProjectCondoManagementAPI.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public CondoMemberDto ToCondoMemberDto(User user)
        {
            var condoMemberDto = new CondoMemberDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId,
                UserId = user.Id
            };
            return condoMemberDto;
        }
    }
}
