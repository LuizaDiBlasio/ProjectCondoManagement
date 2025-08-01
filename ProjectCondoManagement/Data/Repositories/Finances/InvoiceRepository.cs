using ClassLibrary;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;

namespace ProjectCondoManagement.Data.Repositories.Finances
{
    public class InvoiceRepository : GenericRepository<Invoice, DataContextFinances>, IInvoiceRepository
    {
    }
}
