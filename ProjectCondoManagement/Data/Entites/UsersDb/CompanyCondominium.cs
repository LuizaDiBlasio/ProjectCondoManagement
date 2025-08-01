namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class CompanyCondominium
    {
        public int CompanyId { get; set; }

        public Company Company { get; set; }

        public int CondominiumId { get; set; }
    }
}
