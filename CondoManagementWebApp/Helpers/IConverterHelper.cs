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

        PaymentDto FromOneTimeToPaymentDto(CreateOneTimePaymentViewModel model);

        UnitDto ToUnitDto(UnitDtoViewModel model);

        OccurrenceDto ToOccurenceDto(CreateOccurrenceViewModel model);

        OccurrenceDto ToEditedOccurrenceDto(EditOccurrenceViewModel model);

        EditOccurrenceViewModel ToEditOccurrenceView(OccurrenceDto occurrenceDto, List<int> selectedIds);

        List<SelectListItem> ToCondoMembersSelectList(List<CondoMemberDto> condomembersList);

        List<SelectListItem> ToCondosSelectList(List<CondominiumDto> condominiumList);

        MeetingDto ToNewMeetingDto(CreateMeetingViewModel model);

        List<SelectListItem> ToOccurrenceSelectList(List<OccurrenceDto> occurrenceList);

        EditMeetingViewModel ToEditMeetingViewModel(MeetingDto meeting);

        MeetingDto ToEditedMeetingDto(EditMeetingViewModel model);
    }
}
