using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;

namespace ProjectCondoManagement.Data.Repositories.Finances.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice, DataContextFinances>
    {
    }
}
