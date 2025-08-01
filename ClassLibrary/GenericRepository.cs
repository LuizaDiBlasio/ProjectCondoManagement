using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class GenericRepository<T, TContext> : IGenericRepository<T, TContext> where T : class, IEntity where TContext : DbContext

    {

        /// <summary>
        /// Asynchronously adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A "Task" that represents the asynchronous save operation.</returns>
        public async Task CreateAsync(T entity, TContext context)
        {
            await context.Set<T>().AddAsync(entity);

            await SaveAllAsync(context);
        }


        /// <summary>
        /// Asynchronously deletes an existing entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>A "Task" that represents the asynchronous save operation.</returns>
        public async Task DeleteAsync(T entity, TContext context)
        {
            context.Set<T>().Remove(entity);

            await SaveAllAsync(context);
        }


        /// <summary>
        /// Asynchronously checks if an entity with the specified ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the entity to check.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation.
        /// The task result contains "true" if the entity exists, otherwise "false".
        /// </returns>
        public async Task<bool> ExistAsync(int id, TContext context)
        {
            return await context.Set<T>().AnyAsync(e => e.Id == id);
        }


        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database as a queryable object.
        /// The entities are returned as no-tracking, meaning they are not tracked by the change tracker.
        /// </summary>
        /// <returns>An IQueryable{T}" that represents all entities of the specified type.</returns>
        public IQueryable<T> GetAll(TContext context)
        {
            return context.Set<T>().AsNoTracking();
        }


        /// <summary>
        /// Asynchronously retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>
        /// A "Task{TResult}" that represents the asynchronous operation containing the entity if found, otherwise "null".
        /// </returns>
        public async Task<T> GetByIdAsync(int id, TContext context)
        {
            return await context.Set<T>()
                .FirstOrDefaultAsync(e => e.Id == id); //busca a entidade do id dado por parametro;
        }


        /// <summary>
        /// Asynchronously updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A "Task" that represents the asynchronous save operation.</returns>
        public async Task UpdateAsync(T entity, TContext context)
        {
            context.Set<T>().Update(entity);

            await SaveAllAsync(context);
        }

        /// <summary>
        /// Asynchronously saves all pending changes to the database.
        /// </summary>
        /// <returns>
        /// A Task{TResult}" that represents the asynchronous save operation containing "true" if changes were saved successfully, otherwise "false".
        /// </returns>
        private async Task<bool> SaveAllAsync(TContext context)
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
