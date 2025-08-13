using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class DataContextUsers : IdentityDbContext<User>
    {
        public DataContextUsers(DbContextOptions<DataContextUsers> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }

        //public DbSet<CompanyCondominium> CompanyCondominiums { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //chave primária composta
        //    modelBuilder.Entity<CompanyCondominium>()
        //        .HasKey(cc => new { cc.CompanyId, cc.CondominiumId });

        //    //Explicitar relação 1:many Company - CompanyCondominuins
        //    modelBuilder.Entity<Company>()
        //    .HasMany(c => c.CompanyCondominiums) // Uma Company tem MUITAS CompanyCondominium
        //    .WithOne(cc => cc.Company)           // Uma CompanyCondominium tem UMA Company
        //    .HasForeignKey(cc => cc.CompanyId);  // A FK está na CompanyCondominium
        //}
    }
}
