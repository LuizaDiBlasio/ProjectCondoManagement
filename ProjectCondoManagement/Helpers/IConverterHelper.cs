using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IConverterHelper
    {
        CompanyDto? ToCompanyDto(Company company);

        CondoMember ToCondoMember(CondoMemberDto condoMemberDto);

        CondoMemberDto ToCondoMemberDto(CondoMember condoMember, bool includeUnits = true);

       CondoMemberDto ToCondoMemberDto(User user);

        UserDto ToUserDto(User user);

        Task<User> ToEditedUser(EditUserDetailsDto editUserDetailsDto);

        Task<User> ToEditedProfile(UserDto userDto);

        Task<CondoMember> FromUserToCondoMember(User user);

        User ToUser(UserDto userDto);

        Company ToCompany(CompanyDto companyDto, bool isNew);

        CondominiumDto ToCondominiumDto(Condominium condominium);    

        Condominium ToCondominium(CondominiumDto condominiumDto, bool isNew);

        OccurrenceDto ToOccurrenceDto(Occurrence occurrence);

        Occurrence ToOccurrence(OccurrenceDto occurrenceDto, bool isNew);

        Unit ToUnit(UnitDto unitDto, bool isNew);

        Document ToDocument(DocumentDto documentDto, bool isNew);

        DocumentDto ToDocumentDto(Document document);

        MessageDto ToMessageDto(Message message, List<SelectListItem> statusList);

        Message ToMessage(MessageDto messageDto, bool isNew);

        PaymentDto ToPaymentDto(Payment payment, bool isNew);
        UnitDto ToUnitDto(Unit unit, bool includeCondoMembers = true);

        InvoiceDto ToInvoiceDto(Invoice invoice, bool isNew);

        ExpenseDto ToExpenseDto(Expense expense, bool isNew);

        TransactionDto ToTransactionDto(Transaction transaction, bool isNew);

        FinancialAccountDto ToFinancialAccountDto(FinancialAccount financialAccount, bool isNew);

        Expense ToExpense(ExpenseDto expenseDto, bool isNew);

        Payment ToPayment(PaymentDto paymentDto, bool isNew);

        Transaction ToTransaction(TransactionDto transactionDto, bool isNew);

        public FinancialAccount ToFinancialAccount(FinancialAccountDto financialAccountDto, bool isNew);
    }
}
