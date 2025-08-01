using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IGenericRepository<T, TContext> where T : class where TContext : DbContext

    {
        IQueryable<T> GetAll(TContext context); // devolve todas os objetos de uma dada classe

        Task<T> GetByIdAsync(int id, TContext context); //Id definido pelo IEntity

        Task CreateAsync(T entity, TContext context); //cria uma entidade qualquer

        Task UpdateAsync(T entity, TContext context); //faz update de uma entidade qualquer

        Task DeleteAsync(T entity, TContext context);  // deleta uma entidade qualquer

        Task<bool> ExistAsync(int id, TContext context); // ver se existem objetos de uma entitade qualquer
    }
}
