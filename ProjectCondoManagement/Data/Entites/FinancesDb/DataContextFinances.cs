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

            //desabilitar cascata
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes() //buscar todas as entidades
                .SelectMany(t => t.GetForeignKeys()) //selecionar todas as chaves estrangeiras 
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade); // que tenham comportamento em cascata (relações com outras tabelas)

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict; // restringe comportamento ao deletar, se houver entidades filhas, não deleta. Deve ser deletado por código individualmente.
            }

            // Mapeamento do relacionamento 1:1 entre Payment e Invoice
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                    .WithOne()
                .HasForeignKey<Invoice>(i => i.PaymentId) // A chave estrangeira  em Invoice (lado dependente)
                .IsRequired(false); //  um Payment pode existir sem uma Invoice (Invoice é nullable).

            // Mapeamento do relacionamento 1:1 entre Payment e Transaction
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Transaction)
                    .WithOne()
                .HasForeignKey<Transaction>(t => t.PaymentId) // A chave estrangeira em Transaction (lado dependente)
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
