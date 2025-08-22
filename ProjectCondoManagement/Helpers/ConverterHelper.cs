using ClassLibrary.DtoModels;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.Enums;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using System.Threading.Tasks;

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
                FullName = condoMemberDto.FullName,
                Email = condoMemberDto.Email,
                Address = condoMemberDto.Address,
                BirthDate = condoMemberDto.BirthDate,
                PhoneNumber = condoMemberDto.PhoneNumber,
                ImageUrl = condoMemberDto.ImageUrl,
            };

            return condoMember;
        }

        public CondoMemberDto ToCondoMemberDto(CondoMember condoMember)
        {
            var condoMemberDto = new CondoMemberDto
            {
                Id = condoMember.Id,
                FullName = condoMember.FullName,
                Email = condoMember.Email,
                Address = condoMember.Address,
                BirthDate = condoMember.BirthDate,
                PhoneNumber = condoMember.PhoneNumber,
                ImageUrl = condoMember.ImageUrl
            };

            return condoMemberDto;
        }

        public CondoMemberDto ToCondoMemberDto(User user)
        {
            var condoMemberDto = new CondoMemberDto
            {               
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
            };
            return condoMemberDto;
        }

        public UserDto ToUserDto(User user)
        {
            if(user == null)
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
                Email = user.Email,
                CompanyId = user.CompanyId, 
                FinancialAccountId = user.FinancialAccountId,
            };
            return userDto; 
        }

        public User ToUser(UserDto userDto)
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
                CompanyId = userDto.CompanyId,
                FinancialAccountId = userDto.FinancialAccountId,
            };
            return user;    
        }

        public async Task<User> ToEditedUser(EditUserDetailsDto editUserDetailsDto)
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
            user.CompanyId = editUserDetailsDto.CompanyId;  
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

        public  CompanyDto ToCompanyDto(Company company)
        {
            var companyDto = new CompanyDto()
            {
                Id = company.Id,    
                Name = company.Name,
                CondominiumDtos = company.Condominiums?.Select(c => ToCondominiumDto(c)).ToList() ?? new List<CondominiumDto>(),
                CompanyAdmin = ToUserDto(company.CompanyAdmin),
                Email = company.Email,
                Address = company.Address,
                PhoneNumber = company.PhoneNumber,
                TaxIdDocument = company.TaxIdDocument,
                FinancialAccountId = company.FinancialAccountId
            };

            return companyDto;
        }

        public Company ToCompany(CompanyDto companyDto, bool isNew)
        {
            var company = new Company()
            {
                Id = isNew ? 0 : companyDto.Id,
                Name = companyDto.Name,
                Condominiums = companyDto.CondominiumDtos?.Select(c => ToCondominium(c, false)).ToList() ?? new List<Condominium>(),
                CompanyAdmin = companyDto.CompanyAdmin == null? null : ToUser(companyDto.CompanyAdmin),
                Email = companyDto.Email,
                Address = companyDto.Address,
                PhoneNumber = companyDto.PhoneNumber,
                TaxIdDocument = companyDto.TaxIdDocument,
                FinancialAccountId = companyDto.FinancialAccountId  
            };

            return company; 
        }

        public Condominium ToCondominium(CondominiumDto condominiumDto, bool isNew)
        {
            var condominium = new Condominium()
            {
                Id = isNew ? 0 : condominiumDto.Id,
                CondoName = condominiumDto.CondoName, 
                CompanyId = condominiumDto.CompanyId,
                Address = condominiumDto.Address,
                ManagerUserId = condominiumDto.ManagerUserId,
                Units = condominiumDto.Units?.Select(u => ToUnit(u,false)).ToList() ?? new List<Unit>(), 
                Documents = condominiumDto.Documents?.Select(d => ToDocument(d,false)).ToList() ?? new List<Document>(),
                Occurrences = condominiumDto.Occurrences?.Select(o => ToOccurrence(o,false)).ToList() ?? new List<Occurrence>()
            };

            return condominium; 
        }

        public CondominiumDto ToCondominiumDto(Condominium condominium)
        {

            var condominiumDto = new CondominiumDto()
            {
                Id = condominium.Id,
                CondoName = condominium.CondoName,    
                CompanyId = condominium.CompanyId.Value,
                Address = condominium.Address,
                ManagerUserId = condominium.ManagerUserId,
                Units = condominium.Units?.Select(u => ToUnitDto(u)).ToList() ?? new List<UnitDto>(),
                Documents = condominium.Documents?.Select(d => ToDocumentDto(d)).ToList() ?? new List<DocumentDto>(),
                Occurrences = condominium.Occurrences?.Select(o => ToOccurrenceDto(o)).ToList() ?? new List<OccurrenceDto>()
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
                Meeting = occurrence.Meeting
            };

            return occurrenceDto;
        }

        public Occurrence ToOccurrence(OccurrenceDto occurrenceDto, bool isNew)
        {
            var occurence = new Occurrence()
            {
                Id = isNew ? 0 : occurrenceDto.Id,
                Details = occurrenceDto.Details,
                DateAndTime = occurrenceDto.DateAndTime,
                Units = occurrenceDto.UnitDtos?.Select(u => ToUnit(u, false)).ToList() ?? new List<Unit>(),
                Meeting = occurrenceDto.Meeting
            };
            return occurence;
        }
      

        public Unit ToUnit(UnitDto unitDto, bool isNew)
        {
            var unit = new Unit()
            {
                Id = isNew ? 0 : unitDto.Id,
                Condominium = ToCondominium(unitDto.CondominiumDto, false),
                Floor = unitDto.Floor,
                Door = unitDto.Door,
                Occurrences = unitDto.OccurrenceDtos?.Select(o => ToOccurrence(o, false)).ToList() ?? new List<Occurrence>(),
                CondoMembers = unitDto.CondoMemberDtos?.Select(c => ToCondoMember(c)).ToList() ?? new List<CondoMember>()
            };

            return unit;
        }

        public UnitDto ToUnitDto(Unit unit)
        {
            var unitDto = new UnitDto()
            {
                Id = unit.Id,
                CondominiumDto = ToCondominiumDto(unit.Condominium),
                Floor = unit.Floor,
                Door = unit.Door,
                OccurrenceDtos = unit.Occurrences?.Select(o => ToOccurrenceDto(o)).ToList() ?? new List<OccurrenceDto>(),
                CondoMemberDtos = unit.CondoMembers?.Select(c => ToCondoMemberDto(c)).ToList() ?? new List<CondoMemberDto>()
            };

            return unitDto;
        }

        public Document ToDocument(DocumentDto documentDto, bool isNew)
        {
            var document = new Document()
            {
                Id = isNew ? 0 : documentDto.Id,
                CondominiumId = documentDto.CondominiumId,
                FileName = documentDto.FileName,
                ContentType = documentDto.ContentType,
                DocumentUrl = documentDto.DocumentUrl,
                DataUpload = documentDto.DataUpload
            };
            return document;    
        }

        public DocumentDto ToDocumentDto(Document document)
        {
            var documentDto = new DocumentDto()
            {
                Id = document.Id,
                CondominiumId = document.CondominiumId,
                FileName = document.FileName,
                ContentType = document.ContentType,
                DocumentUrl = document.DocumentUrl,
                DataUpload = document.DataUpload
            };
            return documentDto;
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
                PaymentMethod = payment.PaymentMethod,  
                UserEmail = payment.UserEmail,  
                IsPaid = payment.IsPaid,    
                InvoiceDto = ToInvoiceDto(payment.Invoice, false),
                ExpensesDto = payment.Expenses?.Select(e => ToExpenseDto(e, false)).ToList() ?? new List<ExpenseDto>(),
                TransactionDto = ToTransactionDto(payment.Transaction, false),
                
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
                AccountId = invoice.AccountId,
                FinancialAccountDto = ToFinancialAccountDto(invoice.FinancialAccount, false),
                UserEmail = invoice.UserEmail,
                Payment = ToPaymentDto(invoice.Payment, false)
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
                ExpenseTypeDto = new EnumDto { Name = expense.ExpenseType.ToString(), Value = (int)expense.ExpenseType },
            };
           return expenseDto;   
        }

        public TransactionDto ToTransactionDto(Transaction transaction, bool isNew)
        {
            var transactionDto = new TransactionDto()
            {
                Id = isNew ? 0 : transaction.Id,
                DateAndTime = transaction.DateAndTime,
                PayerAccountId = transaction.PayerAccountId,
                AccountPayerDto = ToFinancialAccountDto(transaction.AccountPayer, false),   
                BeneficiaryAccountId = transaction.BeneficiaryAccountId,
                AccountBeneficiaryDto = ToFinancialAccountDto(transaction.AccountBeneficiary, false),
                CondominiumId = transaction.CondominiumId,
                PaymentDto = ToPaymentDto(transaction.Payment, false),
                
            };
            
            return transactionDto;  
        }

        public FinancialAccountDto ToFinancialAccountDto(FinancialAccount financialAccount, bool isNew)
        {
            var financialAccountDto = new FinancialAccountDto()
            {
                Id = isNew ? 0 : financialAccount.Id,
                InitialDeposit = financialAccount.InitialDeposit,
                IsActive = financialAccount.IsActive,
                CardNumber = financialAccount.CardNumber,
                AssociatedBankAccount = financialAccount.AssociatedBankAccount,
                BankName = financialAccount.BankName,
                TransactionsAsBeneficiaryDto = financialAccount.TransactionsAsBeneficiary?.Select(tb => ToTransactionDto(tb, false)).ToList() ?? new List<TransactionDto>(),
                TransactionsAsPayerDto = financialAccount.TransactionsAsPayer?.Select(tp => ToTransactionDto(tp, false)).ToList() ?? new List<TransactionDto>()
            };
            return financialAccountDto; 
        }

    }
    
}
