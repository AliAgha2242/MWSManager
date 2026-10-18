using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Infrastructure.DataBaseRepository
{
    public interface IGenericRepository
    {
        Task<T?> GetAsync<T>(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] includes) where T : class;
        Task<List<T>> GetAllAsync<T>(Func<T, bool> func) where T : class;
        Task<List<T>> GetAllAsync<T>(Func<T, bool> func, params Expression<Func<T, object>>[] includes) where T : class;
        Task<List<T>> GetAllAsync<T>() where T : class;

        List<T> GetAll<T>() where T : class;
        List<T> GetAll<T>(params Expression<Func<T, object>>[] includes) where T : class;
        Task<List<T>> GetAllAsync<T>(params Expression<Func<T, object>>[] includes) where T : class;


        T Update<T>(T entity, bool Concurrency = false) where T : class;
        Task DeleteAsync<T>(T entity) where T : class;
        Task DeleteAsync<T>(Expression<Func<T, bool>> func) where T : class;

        Task DeleteAllAsync<T>(Expression<Func<T, bool>> func);
        void Delete<T>(T entity) where T : class;
        void DeleteAll<T>(Expression<Func<T, bool>> func);
        void Create<T>(T entity) where T : class;
        Task CreateAsync<T>(T entity) where T : class;
        Task SaveChangeAsync();
        Task<bool> ExistAsync<T>(Expression<Func<T, bool>> func) where T : class;
        Task<bool> ExistAsync<T>(T Id) where T : class;

    }



    public class GenericRepository : IGenericRepository
    {
        private readonly MinooDbContext dbContext;
        private readonly IServiceProvider serviceProvider;
        public GenericRepository(MinooDbContext dbContext, IServiceProvider serviceProvider)
        {
            this.dbContext = dbContext;
            this.serviceProvider = serviceProvider;
        }

        public void Delete<T>(T entity) where T : class
        {

            dbContext.Remove(entity);
        }

        public void DeleteAll<T>(Expression<Func<T, bool>> func)
        {
            dbContext.RemoveRange(func);
        }

        public async Task<T?> GetAsync<T>(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] includes) where T : class
        {

            DbSet<T>? result = dbContext.Set<T>();
            foreach (var item in includes)
            {
                result.Include(item);
            }
            return result.FirstOrDefault(func);
        }

        public async Task<List<T>> GetAllAsync<T>() where T : class
        {

            return dbContext.Set<T>().AsNoTracking().ToList();

        }


        public T? Get<T>(Expression<Func<T, bool>> func) where T : class
        {
            return dbContext.Set<T>().AsNoTracking().AsQueryable().FirstOrDefault(func);
        }



        public async Task DeleteAsync<T>(T entity) where T : class
        {
            await Task.Run(async () =>
            {
                if (await this.ExistAsync(entity))
                    dbContext.Remove(entity);
            });
        }

        public Task DeleteAllAsync<T>(Expression<Func<T, bool>> func)
        {

            return Task.Run(() =>
            {
                dbContext.RemoveRange(func);
            });
        }

        public void Create<T>(T entity) where T : class
        {
            dbContext.Set<T>().Add(entity);
        }

        public async Task CreateAsync<T>(T entity) where T : class
        {
            await dbContext.Set<T>().AddAsync(entity);

        }

        public async Task SaveChangeAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync<T>(Expression<Func<T, bool>> func) where T : class
        {
            return await dbContext.Set<T>().AnyAsync(func);
        }

        public async Task<bool> ExistAsync<T>(T entity) where T : class
        {
            var entityType = dbContext.Model.FindEntityType(typeof(T));

            var primaryKey = entityType?.FindPrimaryKey();

            if (primaryKey == null)
                throw new InvalidOperationException(
                    $"Entity {typeof(T).Name} does not have a primary key.");

            var keyProperty = primaryKey.Properties.Single();

            var keyValue = keyProperty.PropertyInfo?.GetValue(entity);

            if (keyValue == null)
                return false;

            var existingEntity = await dbContext.Set<T>().FindAsync(keyValue);

            return existingEntity != null;
        }

        public List<T> GetAll<T>() where T : class
        {
            return dbContext.Set<T>().AsNoTracking().ToList();
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> func) where T : class
        {
            await Task.Run(async () =>
            {
                var result = await GetAsync(func);
                dbContext.Set<T>().Remove(result);
            });
        }

        public async Task<List<T>> GetAllAsync<T>(Func<T, bool> func) where T : class
        {
            return await Task.Run(() =>
            {
                return dbContext.Set<T>().Where(func).ToList();
            });
        }

        public async Task<List<T>> GetAllAsync<T>(Func<T, bool> func, params Expression<Func<T, object>>[] includes) where T : class
        {
            return await Task.Run(() =>
             {
                 DbSet<T>? result = dbContext.Set<T>();
                 foreach (var item in includes)
                 {
                     result.Include(item);
                 }
                 return result.ToList();
             });
        }

        public List<T> GetAll<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            //var res = dbContext.RabbitTasks.Include(p => p.RabbitTaskServerss).Include(p => p.rabbitConsumerRequestHub).ToList();
            //return null;

            IQueryable<T>? result = dbContext.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }
            return result.ToList();
        }

        public T Update<T>(T entity, bool Concurrency = false) where T : class
        {
            try
            {

                if (Concurrency)
                {
                    MinooDbContext context = serviceProvider.GetRequiredService<MinooDbContext>();
                    context.Update(entity);
                    context.SaveChanges();
                    Console.WriteLine("concurency = true");
                }
                else
                {
                    dbContext.Update(entity);
                    Console.WriteLine("concurency = false");
                }
                Console.WriteLine("Ever thing is OK");
                return entity;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return entity;

            }
        }

        public async Task<List<T>> GetAllAsync<T>(params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T>? result = dbContext.Set<T>().AsQueryable();
            foreach (var item in includes)
            {
                result = result.Include(item);
            }
            return await result.ToListAsync();
        }
    }
}

