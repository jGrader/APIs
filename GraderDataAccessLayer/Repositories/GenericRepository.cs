﻿namespace GraderDataAccessLayer.Repositories
{
    using Interfaces;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
  
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DatabaseContext Context;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly Logger Logger;

        protected GenericRepository(DatabaseContext context)
        {
            this.Context = context;
            this.DbSet = this.Context.Set<TEntity>();
            this.Logger = new Logger();
        }

        public async virtual Task<IEnumerable<TEntity>> GetAll()
        {
            var result = await Task.Run(() => DbSet.AsEnumerable());
            return result;
        }

        public async virtual Task<IEnumerable<TEntity>> GetByExpression(Expression<Func<TEntity, bool>> exp)
        {
            var result = await Task.Run(() => DbSet.Where(exp).AsEnumerable());
            return result;
        }

        public async virtual Task<TEntity> Get(object id)
        {
            var result = await Task.Run(() => DbSet.Find(id));
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
                var nonVirtualProperties = entity.GetType().GetProperties().Where(p => !p.GetGetMethod().IsVirtual);

                var tmp = DbSet.AsEnumerable();
                foreach (var prop in nonVirtualProperties)
                {
                    if (prop.Name == "Id")
                    {
                        continue;
                    }
                    var property = prop; // Something in the compiler makes this necessary
                    tmp = tmp.Where(p => property.GetValue(p).Equals(property.GetValue(entity)));
                }

                var enumerable = tmp as IList<TEntity> ?? tmp.ToList();
                if (enumerable.Any())
                {
                    return enumerable.FirstOrDefault();
                }

                DbSet.AddOrUpdate(entity);
                await Context.SaveChangesAsync();

                // The new entry is recorded in the database; load virtual properties before returning
                var virtualProperties = entity.GetType().GetProperties().Where(p => p.GetGetMethod().IsVirtual);
                foreach (var property in virtualProperties)
                {
                    if (property.PropertyType.Name == "ICollection`1")
                    {
                        Context.Entry(entity).Collection(property.Name).Load();
                    }
                    else
                    {
                        Context.Entry(entity).Reference(property.Name).Load();
                    }
                }

                return entity;
            }
            catch (DbException e)
            {
                Logger.Log(e);
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
                DbSet.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                await Context.SaveChangesAsync();
                return entity;
            }
            catch (DbException e)
            {
                Logger.Log(e);
                return null;
            }
        }

        public async virtual Task<bool> Delete(object id)
        {
            try
            {
                var entity = DbSet.Find(id);
                return await Delete(entity);
            }
            catch (Exception e)
            {
                Logger.Log(e);
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
                DbSet.Remove(entity);
                await Context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }
    }
}
