﻿namespace GraderDataAccessLayer.Repositories
{
    using System.Data.Common;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Reflection;
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
                var properties = entity.GetType().GetProperties().Where(p => !p.GetGetMethod().IsVirtual);

                var tmp = dbSet.AsEnumerable();
                foreach (PropertyInfo prop in properties)
                {
                    if (prop.Name == "Id")
                    {
                        continue;
                    }
                    var property = prop; // Something in the compiler makes this necessary
                    tmp = tmp.Where(p => property.GetValue(p).Equals(property.GetValue(entity)));
                }

                if (tmp.Any())
                {
                    return tmp.FirstOrDefault();
                }
                dbSet.AddOrUpdate(entity);
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
