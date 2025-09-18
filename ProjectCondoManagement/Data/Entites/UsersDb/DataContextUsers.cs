using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProjectCondoManagement.Data.Entites.UsersDb
{
    public class DataContextUsers : IdentityDbContext<User>
    {
        public DataContextUsers(DbContextOptions<DataContextUsers> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);

            // A relação é HasMany(u => u.Company) e WithMany() no lado Company
            modelBuilder.Entity<User>()
                   .HasMany(u => u.Companies)
                   .WithMany(c => c.Users);

            //desabilitar cascata
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes() //buscar todas as entidades
                .SelectMany(t => t.GetForeignKeys()) //selecionar todas as chaves estrangeiras 
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade); // que tenham comportamento em cascata (relações com outras tabelas)

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict; // restringe comportamento ao deletar, se houver entidades filhas, não deleta. Deve ser deletado por código individualmente.
            }
        }
    }
}
