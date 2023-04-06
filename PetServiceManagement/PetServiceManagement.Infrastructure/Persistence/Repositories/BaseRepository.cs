using PetServiceManagement.Infrastructure.Persistence.Entities;
using System.Threading.Tasks;

namespace PetServiceManagement.Infrastructure.Persistence.Repositories
{
    public class BaseRepository
    {
        /// <summary>
        /// base on number of records (count) and page size, return total number of pages
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected int GetTotalPages(int count, int pageSize)
        {
            var fullPages = count / pageSize; //full pages with example 23 count and offset is 10. we will get 2 full pages (10 each page)
            var remaining = count % pageSize; //remaining will be 3 which will be an extra page
            var totalPages = (remaining > 0) ? fullPages + 1 : fullPages; //therefore total pages is sum of full pages plus one more page is any remains.

            return totalPages;
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
