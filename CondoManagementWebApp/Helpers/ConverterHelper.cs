using ClassLibrary.DtoModels;
using CondoManagementWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                Id = model.Id,
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
                Id = userDto.Id,
                FullName = userDto.FullName,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                ImageUrl = userDto.ImageUrl,
                Email = userDto.Email
            };

            return model;   
        }

        public EditUserDetailsViewModel ToEditUserDetailsViewModel (UserDto userDto)
        {
            var model = new EditUserDetailsViewModel()
            {
                Id = userDto.Id,
                FullName = userDto.FullName,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                ImageUrl = userDto.ImageUrl,
                IsActive = userDto.IsActive,
                Email = userDto.Email
            };

            return model;
        }

        public EditUserDetailsDto ToEditUserDetailsDto (EditUserDetailsViewModel model)
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
                FinancialAccountId = model.FinancialAccountId, 
               
            };

            return editUserDetailsDto;
        }

        public CompanyDto ToCompanyDto(CreateEditCompanyViewModel model)
        {
            
            var companyDto = new CompanyDto()
            {
                Id = model.Id,
                Name = model.Name, 
                Email = model.Email,
                FinancialAccountId = model.FinancialAccountId,
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
                Email = editedCompany.Email,
                Address = editedCompany.Address,
                FinancialAccountId = editedCompany.FinancialAccountId, 
                PhoneNumber = editedCompany.PhoneNumber,
                TaxIdDocument = editedCompany.TaxIdDocument
            };

            return model;   
        }

        public MessageDto ToMessageDto(CreateMessageViewModel model, DateTime date, string email, EnumDto status)
        {
            var messageDto = new MessageDto()
            {
                Id = model.Id,
                MessageTitle = model.MessageTitle,
                MessageContent = model.MessageContent,
                ReceiverEmail = model.ReceiverEmail,
                PostingDate = date,
                SenderEmail = email,
                Status = status
            };

            return messageDto;
        }

       

        public PaymentDto FromOneTimeToPaymentDto(CreateOneTimePaymentViewModel model)
        {
            if (model.SelectedPayerType == "Condominium")
            {
                string Recipient = model.Recipient ?? string.Empty;
            }



                var paymentDto = new PaymentDto()
                {
                    DueDate = model.DueDate.Value,
                    PayerFinancialAccountId = model.PayerFinancialAccountId.Value,
                    CondominiumId = model.CondominiumId.Value,
                    Recipient = model.Recipient,
                    Amount = model.ExpenseAmount,
                };

            return paymentDto;
        }

        


        public UnitDto ToUnitDto(UnitDtoViewModel model)
        {
            var unitDto = new UnitDto()
            {
                Id = model.Id,
                CondominiumId = (model.CondominiumId),
                Bedrooms = model.Bedrooms,
                Floor = model.Floor,
                Door = model.Door,
            };

            return unitDto;
        }

        public OccurrenceDto ToOccurenceDto(CreateOccurrenceViewModel model)
        {
            var ocurrenceDto = new OccurrenceDto()
            {
                Id = 0,
                Details = model.Details,
                DateAndTime = model.DateAndTime.Value,
                Subject = model.Subject,
                CondominiumId = model.CondominiumId.Value
            };
            return ocurrenceDto;    
        }

        public OccurrenceDto ToEditedOccurrenceDto(EditOccurrenceViewModel model)
        {
            var occurrenceDto = new OccurrenceDto()
            {
                Id = model.Id,
                Details = model.Details,
                DateAndTime = model.DateAndTime.Value,
                Subject = model.Subject,  
                CondominiumId= model.CondominiumId.Value    
            };
            return occurrenceDto;   
        }

        public EditOccurrenceViewModel ToEditOccurrenceView(OccurrenceDto occurrenceDto, List<int> selectedIds)
        {
           
            var model = new EditOccurrenceViewModel()
            {
                Details = occurrenceDto.Details,
                Subject = occurrenceDto.Subject,
                SelectedUnitIds = selectedIds,
                IsResolved = occurrenceDto.IsResolved,
                DateAndTime = occurrenceDto.DateAndTime,
                Id = occurrenceDto.Id,
                CondominiumId = occurrenceDto.CondominiumId,    
            };
            return model;
        }

        public List<SelectListItem> ToCondoMembersSelectList(List<CondoMemberDto> condomembersList)
        {
            return condomembersList.Select(cm => new SelectListItem
            {
                Value = cm.Id.ToString(),
                Text = cm.ToString()
            }).ToList();
        }

        public List<SelectListItem> ToOccurrrencesSelectList(List<OccurrenceDto> occurrencesList)
        {
            return occurrencesList.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.ToString()
            }).ToList();
        }

        public List<SelectListItem> ToCondosSelectList(List<CondominiumDto> condominiumList)
        {
            return condominiumList.Select(cm => new SelectListItem
            {
                Value = cm.Id.ToString(),
                Text = cm.CondoName.ToString()
            }).ToList();
        }

        public List<SelectListItem> ToOccurrenceSelectList(List<OccurrenceDto> occurrenceList)
        {
            var selectList = occurrenceList.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.Subject.ToString()
            }).ToList();

            selectList.Add(new SelectListItem() {Value = "0", Text = "Others" });

            selectList.OrderByDescending(o => o.Value).ToList();

            return selectList;
        }

        public MeetingDto ToNewMeetingDto(CreateMeetingViewModel model)
        {
            var meetignDto = new MeetingDto()
            {
                Id = 0, 
                CondominiumId = model.CondominiumId.Value,    
                DateAndTime = model.DateAndTime.Value, 
                Title = model.Title,
                Description = model.Description,
                IsExtraMeeting = model.IsExtraMeeting,

            };
            return meetignDto;  
        }

        public EditMeetingViewModel ToEditMeetingViewModel(MeetingDto meeting)
        {
            var model = new EditMeetingViewModel()
            {
                Id = meeting.Id,
                CondominiumId = meeting.CondominiumId,
                DateAndTime = meeting.DateAndTime,
                Title = meeting.Title,  
                Description = meeting.Description,  
                MeetingType = meeting.IsExtraMeeting,
            };
            return model;   
        }


        public void SetEditedMeetingProperties(MeetingDto meetingDto, List<CondoMemberDto> condoMembersDto, List<OccurrenceDto> occurrencesDto, EditMeetingViewModel model)
        {
            meetingDto.CondoMembersDto = condoMembersDto;
            meetingDto.OccurencesDto = occurrencesDto;
            meetingDto.Title = model.Title;
            meetingDto.IsExtraMeeting = model.MeetingType;
            meetingDto.CondominiumId = model.CondominiumId.Value;
            meetingDto.DateAndTime = model.DateAndTime.Value;
            meetingDto.Description = model.Description;
          
        }

        public List<SelectListItem> ToCompaniesSelectList(List<CompanyDto> companies)
        {
            return companies.Select(cm => new SelectListItem
            {
                Value = cm.Id.ToString(),
                Text = cm.Name.ToString()
            }).ToList();
        }

    }
}
