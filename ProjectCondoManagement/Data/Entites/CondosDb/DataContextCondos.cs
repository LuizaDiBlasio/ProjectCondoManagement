using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProjectCondoManagement.Data.Entites.CondosDb
{
    public class DataContextCondos : DbContext
    {
        public DataContextCondos(DbContextOptions<DataContextCondos> options) : base(options)
        {
        }

        public DbSet<CondoMember> CondoMembers { get; set; }

        public DbSet<Condominium> Condominiums { get; set; }

        public DbSet<Meeting> Meeting { get; set; }

        public DbSet<Occurrence> Occurences { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<Vote> Votes { get; set; }

        public DbSet<Voting> Voting { get; set; }
        public DbSet<Document> Documents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // configura tabelas padrão do Identity

            //antes de criar o modelo
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
