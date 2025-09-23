namespace ProjectCondoManagement.Helpers
{
    public interface IFinancialAccountHelper
    {
        Task UpdateFinancialAccountNameAsync(int accountId, string ownerName);
    }
}
