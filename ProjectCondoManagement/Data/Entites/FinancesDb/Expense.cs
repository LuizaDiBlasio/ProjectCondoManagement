using ClassLibrary;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Expense : IEntity
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Detail { get; set; }

        public int CondominiumId {  get; set; } 

        public ExpenseType ExpenseType { get; set; }
    }
}
