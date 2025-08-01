using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class PaymentRepository : GenericRepository<Payment, DataContextFinances>, IPaymentRepository
    {
    }
}
