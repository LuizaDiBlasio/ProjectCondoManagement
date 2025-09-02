using ClassLibrary;
using ProjectCondoManagement.Data.Entites.Enums;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Expense : IEntity
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Detail { get; set; }

        public ExpenseType ExpenseType { get; set; }

        public int CondominiumId { get; set; }
    }
}
