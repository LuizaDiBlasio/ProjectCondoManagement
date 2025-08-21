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

        public DbSet<Message> Messages { get; set; }


    }
}
