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
            // Converter para List<int> <----> string separada por vírgulas (assim consigo gravar a lista na base de dados)
            var converter = new ValueConverter<List<int>, string>(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            );

            modelBuilder.Entity<Company>()
                .Property(e => e.CondominiumIds)
                .HasConversion(converter);

            //Para fugir do problema de migração
            // O EF Core verá User.CompanyId como um FK para Company.Id.
            // Mesmo que seja apenas o Admin, o EF precisa dessa definição para a FK User.CompanyId.
            // A relação é HasOne(u => u.Company) e WithMany() no lado Company (sem navegação explícita no Company)
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
               .HasOne(u => u.Company)
               .WithMany()
               .HasForeignKey(u => u.CompanyId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

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
