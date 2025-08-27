using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CondoManagementWebApp.Helpers
{
    public interface IConverterHelper
    {
        LoginDto ToLoginDto(LoginViewModel model);

        RegisterUserDto ToRegisterDto(RegisterUserViewModel model);

        ResetPasswordDto ToResetPasswordDto(ResetPasswordViewModel model); //userId token

        RegisterUserDto ToRegisterDto(CondoMemberDto condoMember);

        ChangePasswordDto ToChangePasswordDto(ChangePasswordViewModel model);

        UserDto ToUserDto(ProfileViewModel model);
            
        ProfileViewModel ToProfileViewModel(UserDto userDto);

        EditUserDetailsViewModel ToEditUserDetailsViewModel(UserDto userDto, string? companyName);

        EditUserDetailsDto ToEditUserDetailsDto(EditUserDetailsViewModel model, string? companyName);

        CompanyDto ToCompanyDto(CreateEditCompanyViewModel model);

        CreateEditCompanyViewModel ToCreateEditCompanyViewModel(CompanyDto editedCompany);

        MessageDto ToMessageDto(CreateMessageViewModel model, DateTime date, string email, EnumDto status);

        ExpenseDto ToExpenseDto(CreateEditExpenseViewModel model);

        PaymentDto FromOneTimeToPaymentDto(CreateOneTimePaymentViewModel model);

        PaymentDto FromRecurringToPaymentDto(CreateRecurringPaymentViewModel model);
        UnitDto ToUnitDto(UnitDtoViewModel model);
    }
}
