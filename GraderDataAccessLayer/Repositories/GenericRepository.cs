namespace GraderDataAccessLayer.Repositories
{
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq.Expressions;

    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbContext context;
        protected readonly DbSet<TEntity> dbSet;

        protected GenericRepository(DbContext context)
        {
            this.context = context;
            dbSet = this.context.Set<TEntity>();
        }

        public async virtual Task<IEnumerable<TEntity>> GetAll()
        {
            var result = await Task.Run(() => dbSet.AsEnumerable());
            return result;
        }

        public async virtual Task<IEnumerable<TEntity>> GetByExpression(Expression<Func<TEntity, bool>> exp)
        {
            var result = await Task.Run(() => dbSet.Where(exp).AsEnumerable());
            return result;
        }

        public async virtual Task<TEntity> Get(object id)
        {
            var result = await Task.Run(() => dbSet.Find(id));
            return result;
        }

        public async virtual Task<TEntity> Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                dbSet.Add(entity);
                await context.SaveChangesAsync();
                return entity;
            }
            catch (DbException)
            {
                return null;
            }
            
        }

        public async virtual Task<TEntity> Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return entity;
            }
            catch (DbException)
            {
                return null;
            }
            
        }

        public async virtual Task<bool> Delete(object id)
        {
            try
            {
                var entity = dbSet.Find(id);
                return await Delete(entity);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async virtual Task<bool> Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                dbSet.Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
