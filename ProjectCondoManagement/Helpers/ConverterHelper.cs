using ClassLibrary.DtoModels;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
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

        public CondominiumDto ToCondominiumDto(Condominium condominium)
        {
            var condominiumDto = new CondominiumDto
            {
                Id = condominium.Id,
                CondoName = condominium.CondoName,
                CompanyId = condominium.CompanyId,
                Address = condominium.Address,
                ManagerUserId = condominium.ManagerUserId,
                Units = condominium.Units?.Select(u => ToUnitDto(u)).ToList() ?? new List<UnitDto>(),
                Documents = condominium.Documents?.Select(d => ToDocumentDto(d)).ToList() ?? new List<DocumentDto>(),
                Occurrences = condominium.Occurences?.Select(o => ToOccurrenceDto(o)).ToList() ?? new List<OccurrenceDto>(),
                FinancialAccountId = condominium.FinancialAccountId,
            };

            return condominiumDto;
        }

        private OccurrenceDto ToOccurrenceDto(Occurrence occurrence)
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

        private DocumentDto ToDocumentDto(Document document)
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

        public UnitDto ToUnitDto(Unit unit, bool includeCondoMembers = true)
        {
            var dto = new UnitDto
            {
                Id = unit.Id,
                Bedrooms = unit.Bedrooms,
                CondominiumId = unit.CondominiumId,
                Floor = unit.Floor,
                Door = unit.Door,
                CondominiumDto = ToCondominiumDto(unit.Condominium)
            };

            if (includeCondoMembers)
            {

            }

            return dto;
        }

        public Fee ToFee(FeeDto feeDto)
        {
            var fee = new Fee
            {
                Id = feeDto.Id,
                Name = feeDto.Name,
                Value = feeDto.Value
            };

            return fee;
        }

        public FeeDto ToFeeDto(Fee fee)
        {
            var feeDto = new FeeDto
            {
                Id = fee.Id,
                Name = fee.Name,
                Value = fee.Value,  
            };

            return feeDto;
        }

        public UserDto ToUserDto(User user)
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
                Email = user.Email,
                CompanyId = user.CompanyId,
                FinancialAccountId = user.FinancialAccountId,
            };

            return userDto;
        }

        public TransactionDto ToTransactionDto(Transaction transaction, bool isNew)
        {
            var transactionDto = new TransactionDto()
            {
                Id = isNew ? 0 : transaction.Id,
                DateAndTime = transaction.DateAndTime,
                PayerAccountId = transaction.PayerAccountId,
                AccountPayer = ToFinancialAccountDto(transaction.AccountPayer, false),
                BeneficiaryAccountId = transaction.BeneficiaryAccountId,
                AccountBeneficiary = ToFinancialAccountDto(transaction.AccountBeneficiary, false),
                PaymentId = transaction.PaymentId,
            };

            return transactionDto;
        }

        public PaymentDto ToPaymentDto(Payment payment, bool isNew)
        {
            var paymentDto = new PaymentDto()
            {
                Id = isNew ? 0 : payment.Id,
                IssueDate = payment.IssueDate,
                DueDate = payment.DueDate,
                PaymentMethod = payment.PaymentMethod,
                CondominiumId = payment.CondominiumId,
                IsPaid = payment.IsPaid,
                InvoiceDto = payment.Invoice == null ? null : ToInvoiceDto(payment.Invoice, false),
                ExpensesDto = payment.Expenses?.Select(e => ToExpenseDto(e, false)).ToList() ?? new List<ExpenseDto>(),
                OneTimeExpenseDto = payment.OneTimeExpense == null ? null : ToExpenseDto(payment.OneTimeExpense, false),
                TransactionDto = payment.Transaction == null ? null : ToTransactionDto(payment.Transaction, false),

            };

            return paymentDto;
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
            };
            return expenseDto;
        }

        public CompanyDto? ToCompanyDto(Company company)
        {
            var companyDto = new CompanyDto()
            {
                Id = company.Id,
                Name = company.Name,
                CondominiumDtos = company.Condominiums?.Select(c => ToCondominiumDto(c)).ToList() ?? new List<CondominiumDto>(),
                Email = company.Email,
                Address = company.Address,
                PhoneNumber = company.PhoneNumber,
                TaxIdDocument = company.TaxIdDocument,
                FinancialAccountId = company.FinancialAccountId,
                SelectedCondominiumIds = company.CondominiumIds?.ToList() ?? new List<int>(),
                CompanyAdminId = company.CompanyAdminId,

            };

            return companyDto;
        }

    }
}
