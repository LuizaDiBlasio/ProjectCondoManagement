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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<List<int>, string>(
               v => string.Join(",", v),
               v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
           );

            modelBuilder.Entity<Company>()
                .Property(e => e.CondominiumIds)
                .HasConversion(converter);

            base.OnModelCreating(modelBuilder);
        }
    }
}
