using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;
using System.ComponentModel.DataAnnotations;

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
                Requires2FA = model.Requires2FA,    
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
                ImageUrl = model.ImageUrl
            };
            return registerDto;
        }

        public ResetPasswordDto ToResetPasswordDto(ResetPasswordViewModel model)
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                Token = model.Token,
                UserId = model.UserId,
                Password = model.Password
            };

            return resetPasswordDto;
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

        public ChangePasswordDto ToChangePasswordDto(ChangePasswordViewModel model)
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword,
                Email = model.Email,    
            };
            return changePasswordDto;
        }

        public UserDto ToUserDto(ProfileViewModel model)
        {
            var userDto = new UserDto()
            {
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                ImageUrl = model.ImageUrl, 
                Email = model.Email,
               
            };

            return userDto; 
        }

        public ProfileViewModel ToProfileViewModel(UserDto userDto)
        {
            var model = new ProfileViewModel()
            {
                FullName = userDto.FullName,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                ImageUrl = userDto.ImageUrl,
                Email = userDto.Email
            };

            return model;   
        }

        public EditUserDetailsViewModel ToEditUserDetailsViewModel (UserDto userDto, string? companyName)
        {
            var model = new EditUserDetailsViewModel()
            {
                FullName = userDto.FullName,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                ImageUrl = userDto.ImageUrl,
                IsActive = userDto.IsActive,
                Email = userDto.Email,
                CompanyName = companyName,
                CompanyId = userDto.CompanyId,
                FinancialAccountId = userDto.FinancialAccountId
            };

            return model;
        }

        public EditUserDetailsDto ToEditUserDetailsDto (EditUserDetailsViewModel model, string? companyName)
        {
            var editUserDetailsDto = new EditUserDetailsDto()
            {
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Email = model.Email,
                IsActive = model.IsActive,
                ImageUrl = model.ImageUrl,
                CompanyName = model.CompanyName,
                FinancialAccountId = model.FinancialAccountId, 
                CompanyId = model.CompanyId 
            };

            return editUserDetailsDto;
        }

        public CompanyDto ToCompanyDto(CreateEditCompanyViewModel model)
        {
            
            var companyDto = new CompanyDto()
            {
                Id = model.Id,
                Name = model.Name, 
                CompanyAdminId = model.SelectedCompanyAdminId,
                SelectedCondominiumIds = model.SelectedCondominiumIds,
                Email = model.Email,
                Address = model.Address,
                PhoneNumber= model.PhoneNumber, 
                TaxIdDocument = model.TaxIdDocument,    
            };

            return companyDto;
        }

        public CreateEditCompanyViewModel ToCreateEditCompanyViewModel(CompanyDto editedCompany)
        {
            var model = new CreateEditCompanyViewModel()
            {
                Name = editedCompany.Name,
                SelectedCompanyAdminId = editedCompany.CompanyAdminId,
                SelectedCondominiumIds = editedCompany.SelectedCondominiumIds,
                Email = editedCompany.Email,
                Address = editedCompany.Address,
                PhoneNumber = editedCompany.PhoneNumber,
                TaxIdDocument = editedCompany.TaxIdDocument
            };

            return model;   
        }

    }
}
