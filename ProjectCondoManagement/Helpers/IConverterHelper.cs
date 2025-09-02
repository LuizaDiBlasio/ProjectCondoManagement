using ClassLibrary.DtoModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;

namespace ProjectCondoManagement.Helpers
{
    public interface IConverterHelper
    {
        CondoMember ToCondoMember(CondoMemberDto condoMemberDto);
        CondoMemberDto ToCondoMemberDto(User user);
        CondoMemberDto ToCondoMemberDto(CondoMember condoMember);
        CondominiumDto ToCondominiumDto(Condominium condominium);
        Fee ToFee(FeeDto feeDto);
        FeeDto ToFeeDto(Fee fee);
        UserDto ToUserDto(User user);
        CompanyDto? ToCompanyDto(Company company);
        PaymentDto ToPaymentDto(Payment payment, bool isNew);

        UnitDto ToUnitDto(Unit unit, bool includeCondoMembers = true);

        InvoiceDto ToInvoiceDto(Invoice invoice, bool isNew);

        ExpenseDto ToExpenseDto(Expense expense, bool isNew);

        TransactionDto ToTransactionDto(Transaction transaction, bool isNew);

        FinancialAccountDto ToFinancialAccountDto(FinancialAccount financialAccount, bool isNew);

    }
}
