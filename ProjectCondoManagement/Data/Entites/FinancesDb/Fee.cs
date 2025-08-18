using ClassLibrary;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class Fee : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }
    }
}
