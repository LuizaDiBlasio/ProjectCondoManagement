using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace ProjectCondoManagement.Data.Entites.FinancesDb
{
    public class DataContextFinances : DbContext
    {
        public DataContextFinances(DbContextOptions<DataContextFinances> options) : base(options)
        {
        }

        public DbSet<FinancialAccount> FinancialAccounts { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento do relacionamento 1:1 entre Payment e Invoice
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                    .WithOne(i => i.Payment)
                .HasForeignKey<Invoice>(i => i.Id) // A chave estrangeira está em Invoice (lado dependente), e usa a PK de Invoice
                .IsRequired(false); //  um Payment pode existir sem uma Invoice (Invoice é nullable).

            // Mapeamento do relacionamento 1:1 entre Payment e Transaction
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Transaction)
                    .WithOne(i => i.Payment)
                .HasForeignKey<Transaction>(i => i.Id) // A chave estrangeira está em Transaction (lado dependente), e usa a PK de Transaction
                .IsRequired(false); //  um Payment pode existir sem uma Transaction - transação ainda não foi feita (Transaction é nullable).


            // Mapeamento do relacionamento 1:many entre  AccountPayer - TransactionsAsPayer
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.AccountPayer)
                    .WithMany(a => a.TransactionsAsPayer)
                .HasForeignKey(t => t.PayerAccountId)  // FK em Transaction.PayerAccountId
                .OnDelete(DeleteBehavior.Restrict);    // Restringe deleção cascata


            // Mapeamento do relacionamento 1:many entre  AccountBeneficiary - TransactionsAsBanaficiary
            modelBuilder.Entity<Transaction>()
               .HasOne(t => t.AccountBeneficiary)
                   .WithMany(a => a.TransactionsAsBeneficiary)
               .HasForeignKey(t => t.BeneficiaryAccountId) // FK em Transaction.BeneficiaryAccountId
               .OnDelete(DeleteBehavior.Restrict); // Restringe deleção cascata
        }
    }
}
