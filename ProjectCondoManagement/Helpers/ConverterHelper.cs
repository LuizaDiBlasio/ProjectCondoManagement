using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.Enums;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;

namespace ProjectCondoManagement.Helpers
{


    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly ICondoMemberRepository _condoMemberRepository;

        public ConverterHelper(IUserHelper userHelper, ICondoMemberRepository condoMemberRepository)
        {
            _userHelper = userHelper;
            _condoMemberRepository = condoMemberRepository;

        }

        public CondoMember ToCondoMember(CondoMemberDto condoMemberDto)
        {
            var condoMember = new CondoMember
            {
                Id = condoMemberDto.Id,
                FinancialAccountId = condoMemberDto.FinancialAccountId,
                FullName = condoMemberDto.FullName,
                Email = condoMemberDto.Email,
                Address = condoMemberDto.Address,
                BirthDate = condoMemberDto.BirthDate,
                PhoneNumber = condoMemberDto.PhoneNumber,
                ImageUrl = condoMemberDto.ImageUrl
            };

            return condoMember;
        }

        public CondoMemberDto ToCondoMemberDto(CondoMember condoMember, bool includeUnits = true)
        {
            var dto = new CondoMemberDto
            {
                Id = condoMember.Id,
                FullName = condoMember.FullName,
                FinancialAccountId = condoMember.FinancialAccountId,
                Email = condoMember.Email,
                Address = condoMember.Address,
                BirthDate = condoMember.BirthDate,
                PhoneNumber = condoMember.PhoneNumber,
                ImageUrl = condoMember.ImageUrl
            };

            if (includeUnits)
            {
                dto.Units = condoMember.Units?
                    .Select(u => ToUnitDto(u, includeCondoMembers: false))
                    .ToList() ?? new List<UnitDto>();
            }

            return dto;
        }



        public CondoMemberDto ToCondoMemberDto(User user)
        {
            var condoMemberDto = new CondoMemberDto
            {
                FullName = user.FullName,
                FinancialAccountId = user.FinancialAccountId ?? 0,
                Email = user.Email,
                Address = user.Address,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
            };
            return condoMemberDto;
        }

        public UserDto ToUserDto(User user, bool withCompaniesDto)
        {
            if (user == null)
            {
                return null;
            }

            var userDto = new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImageUrl = user.ImageUrl,
                IsActive = user.IsActive,
                CompaniesDto = withCompaniesDto? user.Companies?.Select(c => ToCompanyDto(c, false)).ToList() ?? new List<CompanyDto>() : new List<CompanyDto>(),
                Email = user.Email,
                FinancialAccountId = user.FinancialAccountId,
            };
            return userDto;
        }

        public User ToUser(UserDto userDto, bool withCompanies)
        {
            var user = new User()
            {
                FullName = userDto.FullName,
                BirthDate = userDto.BirthDate,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                ImageUrl = userDto.ImageUrl,
                IsActive = userDto.IsActive,
                Email = userDto.Email,
                FinancialAccountId = userDto.FinancialAccountId,
                Companies = withCompanies ? userDto.CompaniesDto?.Select(c => ToCompany(c, false, false)).ToList() ?? new List<Company>() : new List<Company>(),
            };
            return user;
        }

        public async Task<User> ToEditedUser(EditUserDetailsDto editUserDetailsDto, bool withCompanies)
        {
            var user = await _userHelper.GetUserByEmailAsync(editUserDetailsDto.Email);

            if (user == null)
            {
                return null;
            }

            user.FullName = editUserDetailsDto.FullName;
            user.BirthDate = editUserDetailsDto.BirthDate;
            user.PhoneNumber = editUserDetailsDto.PhoneNumber;
            user.Address = editUserDetailsDto.Address;
            user.ImageUrl = editUserDetailsDto.ImageUrl;
            user.Email = editUserDetailsDto.Email;
            user.IsActive = editUserDetailsDto.IsActive;
            user.FinancialAccountId = editUserDetailsDto.FinancialAccountId;

            return user;
        }

        public async Task<User> ToEditedProfile(UserDto userDto)
        {
            var user = await _userHelper.GetUserByEmailAsync(userDto.Email);

            if (user == null)
            {
                return null;
            }

            user.FullName = userDto.FullName;
            user.BirthDate = userDto.BirthDate;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Address = userDto.Address;
            user.ImageUrl = userDto.ImageUrl;
            user.Email = userDto.Email;

            return user;
        }

        public async Task<CondoMember> FromUserToCondoMember(User user)
        {
            var condoMember = await _condoMemberRepository.GetCondoMemberByEmailAsync(user.Email);

            if (condoMember == null)
            {
                return null;
            }

            condoMember.FullName = user.FullName;
            condoMember.PhoneNumber = user.PhoneNumber;
            condoMember.Address = user.Address;
            condoMember.ImageUrl = user.ImageUrl;
            condoMember.BirthDate = user.BirthDate;

            return condoMember;
        }

        public CompanyDto ToCompanyDto(Company company, bool withUsersDto)
        {
            var companyDto = new CompanyDto()
            {
                Id = company.Id,
                Name = company.Name,
                CondominiumDtos = company.Condominiums?.Select(c => ToCondominiumDto(c, false)).ToList() ?? new List<CondominiumDto>(),
                CondoMemberDtos = company.CondoMembers?.Select(cm => ToCondoMemberDto(cm, true)).ToList() ?? new List<CondoMemberDto>(),
                Email = company.Email,
                Address = company.Address,
                PhoneNumber = company.PhoneNumber,
                TaxIdDocument = company.TaxIdDocument,
                FinancialAccountId = company.FinancialAccountId,
                CompanyAdminId = company.CompanyAdminId,
                Users = withUsersDto? company.Users?.Select(u => ToUserDto(u, false)).ToList() : new List<UserDto>(),  
            };

            return companyDto;
        }

        public Company ToCompany(CompanyDto companyDto, bool isNew, bool withUsers)
        {
            var company = new Company()
            {
                Id = isNew ? 0 : companyDto.Id,
                Name = companyDto.Name,
                Condominiums = companyDto.CondominiumDtos?.Select(c => ToCondominium(c, false)).ToList() ?? new List<Condominium>(),
                Email = companyDto.Email,
                Address = companyDto.Address,
                PhoneNumber = companyDto.PhoneNumber,
                TaxIdDocument = companyDto.TaxIdDocument,
                FinancialAccountId = companyDto.FinancialAccountId,
                Users = withUsers
                        ? companyDto.Users?.Select(c => ToUser(c, false)).ToList() ?? new List<User>()
                        : new List<User>(),
                CompanyAdminId = companyDto.CompanyAdminId?? null,
            };

            return company;
        }

        public Condominium ToCondominium(CondominiumDto condominiumDto, bool isNew)
        {
            var condominium = new Condominium()
            {
                Id = isNew ? 0 : condominiumDto.Id,
                CondoName = condominiumDto.CondoName,
                CompanyId = condominiumDto.CompanyId.Value,
                Address = condominiumDto.Address,
                ManagerUserId = condominiumDto.ManagerUserId,
                Units = condominiumDto.Units?.Select(u => ToUnit(u, false)).ToList() ?? new List<Unit>(),
                Occurrences = condominiumDto.Occurrences?.Select(o => ToOccurrence(o, false)).ToList() ?? new List<Occurrence>(),
                FinancialAccountId = condominiumDto.FinancialAccountId
            };

            return condominium;
        }

        public CondominiumDto ToCondominiumDto(Condominium condominium, bool IncludeOccurrence)
        {

            var condominiumDto = new CondominiumDto()
            {
                Id = condominium.Id,
                CondoName = condominium.CondoName,
                CondoMembers = condominium.CondoMembers?.Select(c => ToCondoMemberDto(c, false)).ToList() ?? new List<CondoMemberDto>(),
                CompanyId = condominium.CompanyId,
                Address = condominium.Address,
                FinancialAccountId = condominium.FinancialAccountId,
                ManagerUserId = condominium.ManagerUserId,
                Occurrences = IncludeOccurrence == true ? condominium.Occurrences?.Select(o => ToOccurrenceDto(o, false)).ToList() ?? new List<OccurrenceDto>() : null
            };

            return condominiumDto;
        }

        public OccurrenceDto ToOccurrenceDto(Occurrence occurrence)
        {
            var occurrenceDto = new OccurrenceDto()
            {
                Id = occurrence.Id,
                Details = occurrence.Details,
                DateAndTime = occurrence.DateAndTime,
                UnitDtos = occurrence.Units?.Select(u => ToUnitDto(u)).ToList() ?? new List<UnitDto>(),
                IsResolved = occurrence.IsResolved,
                CondominiumId = occurrence.CondominiumId,
                Subject = occurrence.Subject,

            };

            return occurrenceDto;
        }

        public Occurrence ToOccurrence(OccurrenceDto occurrenceDto, bool isNew)
        {
            var occurence = new Occurrence()
            {
                Id = isNew ? 0 : occurrenceDto.Id,
                Details = occurrenceDto.Details,
                Subject = occurrenceDto.Subject,
                DateAndTime = occurrenceDto.DateAndTime,
                Units = occurrenceDto.UnitDtos?.Select(u => ToUnit(u, false)).ToList() ?? new List<Unit>(),
                CondominiumId = occurrenceDto.CondominiumId,
            };
            return occurence;
        }


        public Unit ToUnit(UnitDto unitDto, bool isNew)
        {
            var unit = new Unit()
            {
                Id = isNew ? 0 : unitDto.Id,
                CondominiumId = unitDto.CondominiumId,
                Floor = unitDto.Floor,
                Door = unitDto.Door,
                Bedrooms = unitDto.Bedrooms
            };

            return unit;
        }

        public UnitDto ToUnitDto(Unit unit, bool includeCondoMembers = true)
        {
            var dto = new UnitDto
            {
                Id = unit.Id,
                Bedrooms = unit.Bedrooms,
                CondominiumId = unit.CondominiumId,
                Floor = unit.Floor,
                Door = unit.Door,
                CondominiumDto = ToCondominiumDto(unit.Condominium, false)
            };

            if (includeCondoMembers)
            {
                dto.CondoMemberDtos = unit.CondoMembers?
                    .Select(cm => ToCondoMemberDto(cm, includeUnits: false))
                    .ToList() ?? new List<CondoMemberDto>();
            }

            return dto;
        }


        public MessageDto ToMessageDto(Message message, List<SelectListItem>? statusList)
        {
            var messageDto = new MessageDto()
            {
                Id = message.Id,
                PostingDate = message.PostingDate,
                MessageTitle = message.MessageTitle,
                MessageContent = message.MessageContent,
                SenderEmail = message.SenderEmail,
                ReceiverEmail = message.ReceiverEmail,
                Status = new EnumDto { Name = message.Status.ToString(), Value = (int)message.Status },
                StatusList = statusList,
                DeletedBySender = message.DeletedBySender,
                DeletedByReceiver = message.DeletedByReceiver
            };

            return messageDto;
        }

        public Message ToMessage(MessageDto messageDto, bool isNew)
        {
            var message = new Message()
            {
                Id = isNew ? 0 : messageDto.Id,
                PostingDate = messageDto.PostingDate.Value,
                MessageTitle = messageDto.MessageTitle,
                MessageContent = messageDto.MessageContent,
                SenderEmail = messageDto.SenderEmail,
                ReceiverEmail = messageDto.ReceiverEmail,
                Status = (MessageStatus)messageDto.Status.Value,
            };

            return message;
        }

        public PaymentDto ToPaymentDto(Payment payment, bool isNew)
        {
            var paymentDto = new PaymentDto()
            {
                Id = isNew ? 0 : payment.Id,
                IssueDate = payment.IssueDate,
                DueDate = payment.DueDate,
                ExpenseType = payment.ExpenseType,
                Payer = payment.Payer,
                BeneficiaryAccountId = payment.BeneficiaryAccountId,
                SelectedBeneficiaryId = payment.SelectedBeneficiaryId,
                ExternalRecipientBankAccount = payment.ExternalRecipientBankAccount,
                PaymentMethod = payment.PaymentMethod,
                CondominiumId = payment.CondominiumId,
                IsPaid = payment.IsPaid,
                InvoiceDto = payment.Invoice == null ? null : ToInvoiceDto(payment.Invoice, false),
                ExpensesDto = payment.Expenses?.Select(e => ToExpenseDto(e, false)).ToList() ?? new List<ExpenseDto>(),
                OneTimeExpenseDto = payment.OneTimeExpense == null ? null : ToExpenseDto(payment.OneTimeExpense, false),
                PayerFinancialAccountId = payment.PayerFinancialAccountId,
                TransactionDto = payment.Transaction == null ? null : ToTransactionDto(payment.Transaction, false),
                InvoiceId = payment.InvoiceId,
                TransactionId = payment.TransactionId,
                MbwayNumber = payment.MbwayNumber,
                CreditCard = payment.CreditCard,
                Recipient = payment.Recipient,
                Amount = payment.Amount,
            };

            return paymentDto;
        }


        public InvoiceDto ToInvoiceDto(Invoice invoice, bool isNew)
        {
            var invoiceDto = new InvoiceDto()
            {
                Id = isNew ? 0 : invoice.Id,
                PaymentDate = invoice.PaymentDate,
                CondominiumId = invoice.CondominiumId,
                PayerAccountId = invoice.PayerAccountId,
                BeneficiaryAccountId = invoice.BeneficiaryAccountId,
                PaymentId = invoice.PaymentId,
            };

            return invoiceDto;
        }


        public ExpenseDto ToExpenseDto(Expense expense, bool isNew)
        {
            var expenseDto = new ExpenseDto()
            {
                Id = isNew ? 0 : expense.Id,
                Amount = expense.Amount,
                Detail = expense.Detail,
                CondominiumId = expense.CondominiumId,
                ExpenseTypeDto = new EnumDto { Name = expense.ExpenseType.ToString(), Value = (int)expense.ExpenseType },
                PaymentId = expense.PaymentId,
            };
            return expenseDto;
        }

        public TransactionDto ToTransactionDto(Data.Entites.FinancesDb.Transaction transaction, bool isNew)
        {
            var transactionDto = new TransactionDto()
            {
                Id = isNew ? 0 : transaction.Id,
                DateAndTime = transaction.DateAndTime,
                PayerAccountId = transaction.PayerAccountId,
                //AccountBeneficiaryDto = ToFinancialAccountDto(transaction.AccountBeneficiary, false),
                BeneficiaryAccountId = transaction.BeneficiaryAccountId,
                Amount = transaction.Amount,
                CompanyId = transaction?.CompanyId,
                ExternalRecipientBankAccount = transaction.ExternalRecipientBankAccount,
                PaymentId = transaction.PaymentId,


            };

            return transactionDto;
        }

        public FinancialAccountDto ToFinancialAccountDto(FinancialAccount financialAccount, bool isNew)
        {
            var financialAccountDto = new FinancialAccountDto()
            {
                Id = isNew ? 0 : financialAccount.Id,
                OwnerName = financialAccount.OwnerName,
                Balance = financialAccount.Balance,
                IsActive = financialAccount.IsActive,
                CardNumber = financialAccount.CardNumber,
                AssociatedBankAccount = financialAccount.AssociatedBankAccount,
                BankName = financialAccount.BankName,
                TransactionsAsBeneficiaryDto = financialAccount.TransactionsAsBeneficiary?.Select(tb => ToTransactionDto(tb, false)).ToList() ?? new List<TransactionDto>(),
                TransactionsAsPayerDto = financialAccount.TransactionsAsPayer?.Select(tp => ToTransactionDto(tp, false)).ToList() ?? new List<TransactionDto>()
            };
            return financialAccountDto;
        }

        public Expense ToExpense(ExpenseDto expenseDto, bool isNew)
        {
            var expense = new Expense()
            {
                Id = isNew ? 0 : expenseDto.Id,
                Amount = expenseDto.Amount,
                ExpenseType = (ExpenseType)expenseDto.ExpenseTypeDto.Value,
                Detail = expenseDto.Detail,
                CondominiumId = expenseDto.CondominiumId,
                PaymentId = expenseDto.PaymentId
            };

            return expense;
        }

        public Payment ToPayment(PaymentDto paymentDto, bool isNew)
        {
            var payment = new Payment()
            {
                Id = isNew ? 0 : paymentDto.Id,
                IssueDate = paymentDto.IssueDate.Value,
                ExpenseType = paymentDto.ExpenseType,
                Payer = paymentDto.Payer,
                DueDate = paymentDto.DueDate,
                SelectedBeneficiaryId = paymentDto.SelectedBeneficiaryId,
                ExternalRecipientBankAccount = paymentDto.ExternalRecipientBankAccount,
                BeneficiaryAccountId = paymentDto.BeneficiaryAccountId,
                PaymentMethod = isNew ? null : paymentDto.PaymentMethod,
                PayerFinancialAccountId = paymentDto.PayerFinancialAccountId,
                IsPaid = isNew ? false : paymentDto.IsPaid,
                CondominiumId = paymentDto.CondominiumId,
                OneTimeExpense = paymentDto.OneTimeExpenseDto == null ? null : isNew ? ToExpense(paymentDto.OneTimeExpenseDto, true) : ToExpense(paymentDto.OneTimeExpenseDto, false),
                Expenses = paymentDto.ExpensesDto?.Select(e => ToExpense(e, false)).ToList() ?? new List<Expense>(),
                Transaction = isNew ? null : paymentDto.TransactionDto != null ? ToTransaction(paymentDto.TransactionDto, false) : null,
                TransactionId = isNew ? null : paymentDto.TransactionDto == null ? null : paymentDto.TransactionDto.Id,
                InvoiceId = paymentDto.InvoiceId,
                MbwayNumber = paymentDto.MbwayNumber,
                CreditCard = paymentDto.CreditCard,
                Recipient = paymentDto.Recipient,
                Amount = paymentDto.Amount
            };

            return payment;
        }

        public Data.Entites.FinancesDb.Transaction ToTransaction(TransactionDto transactionDto, bool isNew)
        {
            var transaction = new Data.Entites.FinancesDb.Transaction()
            {
                Id = isNew ? 0 : transactionDto.Id,
                DateAndTime = transactionDto.DateAndTime,
                PayerAccountId = transactionDto.PayerAccountId,
                BeneficiaryAccountId = transactionDto.BeneficiaryAccountId,
                PaymentId = transactionDto.PaymentId,
                Amount = transactionDto.Amount,
                CompanyId = transactionDto?.CompanyId,
                ExternalRecipientBankAccount = transactionDto.ExternalRecipientBankAccount,
            };

            return transaction;
        }

        public FinancialAccount ToFinancialAccount(FinancialAccountDto financialAccountDto, bool isNew)
        {
            var financialAccount = new FinancialAccount()
            {
                Id = isNew ? 0 : financialAccountDto.Id,
                OwnerName = financialAccountDto.OwnerName,
                Balance = financialAccountDto.Balance,
                IsActive = financialAccountDto.IsActive,
                CardNumber = financialAccountDto.CardNumber,
                AssociatedBankAccount = financialAccountDto.AssociatedBankAccount,
                BankName = financialAccountDto.BankName,
            };
            return financialAccount;
        }

        public OccurrenceDto ToOccurrenceDto(Occurrence occurrence, bool isNew)
        {
            var occurrenceDto = new OccurrenceDto()
            {
                Id = isNew ? 0 : occurrence.Id,
                Details = occurrence.Details,
                CondominiumId = occurrence.CondominiumId,
                UnitDtos = occurrence.Units?.Select(u => ToUnitDto(u, false)).ToList() ?? new List<UnitDto>(),
                ResolutionDate = occurrence.ResolutionDate,
                DateAndTime = occurrence.DateAndTime,
                IsResolved = occurrence.IsResolved,
                Subject = occurrence.Subject,
            };
            return occurrenceDto;
        }

        public MeetingDto ToMeetingDto(Meeting meeting)
        {
            var meetingDto = new MeetingDto()
            {
                Id = meeting.Id,
                CondominiumId = meeting.CondominiumId,
                DateAndTime = meeting.DateAndTime,
                Title = meeting.Title,
                Description = meeting.Description,
                CondoMembersDto = meeting.CondoMembers?.Select(c => ToCondoMemberDto(c, false)).ToList() ?? new List<CondoMemberDto>(),
                OccurencesDto = meeting.Occurences?.Select(o => ToOccurrenceDto(o, false)).ToList() ?? new List<OccurrenceDto>(),
                MeetingLink = meeting.MeetingLink,
                IsExtraMeeting = meeting.IsExtraMeeting,
                //DocumentDto = ToDocumentDto(meeting.Document)

            };
            return meetingDto;
        }

        public Meeting ToMeeting(MeetingDto meetingDto, bool isNew)
        {
            var meeting = new Meeting()
            {
                Id = isNew ? 0 : meetingDto.Id,
                CondominiumId = meetingDto.CondominiumId,
                DateAndTime = meetingDto.DateAndTime,
                Title = meetingDto.Title,
                Description = meetingDto.Description,
                CondoMembers = meetingDto.CondoMembersDto?.Select(c => ToCondoMember(c)).ToList() ?? new List<CondoMember>(),
                Occurences = meetingDto.OccurencesDto?.Select(o => ToOccurrence(o, false)).ToList() ?? new List<Occurrence>(),
                MeetingLink = meetingDto.MeetingLink,
                IsExtraMeeting = meetingDto.IsExtraMeeting,
            };
            return meeting;
        }
    }

}
