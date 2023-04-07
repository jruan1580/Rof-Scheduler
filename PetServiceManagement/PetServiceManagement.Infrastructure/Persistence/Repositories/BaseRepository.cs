using Microsoft.EntityFrameworkCore;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public class BaseRepository
    {
        protected async Task<List<T>> SkipNAndTakeTopM<T>(IQueryable<T> elements, int skipN, int topM)
        {
            return await elements.Skip(skipN)
                .Take(topM)
                .ToListAsync();
        }

        /// <summary>
        /// Finds the item by the primary key (id).
        /// If found, returns it, otw, returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual async Task<T> GetEntityById<T>(object id)
        {
            using var context = new RofSchedulerContext();
            
            return (T)(await context.FindAsync(typeof(T), id));            
        }

        /// <summary>
        /// creates and entity of some type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual async Task<T> CreateEntity<T>(T entity)
        {
            using var context = new RofSchedulerContext();
            
            await context.AddAsync(entity);

            await context.SaveChangesAsync();

            return entity;            
        }

        /// <summary>
        /// Updates an entity of some type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual async Task UpdateEntity<T>(T entity)
        {
            using var context = new RofSchedulerContext();
            
            context.Update(entity);

            await context.SaveChangesAsync();            
        }

        /// <summary>
        /// Looks for the entity of some Type T by id.
        /// If found, delete it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual async Task DeleteEntity<T>(object id)
        {
            using var context = new RofSchedulerContext();
                
            var entityToDelete = await context.FindAsync(typeof(T), id);

            //does not exist, consider it deleted.
            if (entityToDelete == null)
            {
                return;
            }

            context.Remove(entityToDelete);

            await context.SaveChangesAsync();            
        }       
    }
}
