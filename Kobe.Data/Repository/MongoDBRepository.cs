using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Kobe.Data.Context;
using Kobe.Data.Extensions;
using Kobe.Domain.Collections;
using MongoDB.Driver;

namespace Kobe.Data.Repository
{
    public class MongoDBRepository<TEntity> : IMongoDBRepository<TEntity> where TEntity : class, BaseCollection
    {
        private readonly IMongoCollection<TEntity> dbSet = null;
        private readonly KobeContext _dataContext;
        public MongoDBRepository(KobeContext dataContext)
        {
            _dataContext = dataContext;
            dbSet = _dataContext.GetCollection<TEntity>();
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.CreatedDate = System.DateTime.UtcNow;
            entity.ModifiedDate = System.DateTime.UtcNow;
            await dbSet.InsertOneAsync(entity);
            return entity;
        }

        public async Task<TEntity> Delete(FilterDefinition<TEntity> entity, TEntity replacementModel)
        {
            await dbSet.ReplaceOneAsync(entity, replacementModel);
            return replacementModel;
        }

        public bool Exist(Expression<Func<TEntity, bool>> filter, bool WithDeletedObjects = false)
        {
            return GetQuery(filter, WithDeletedObjects).Any();
        }

        public IQueryable<TEntity> GetAll(bool WithDeletedObjects = false)
        {
            return GetQuery(WithDeletedObjects: WithDeletedObjects);
        }

        public IQueryable<TEntity> GetById(Expression<Func<TEntity, bool>> whereCondition, bool WithDeletedObjects = false)
        {
            return GetQuery(filter: whereCondition, WithDeletedObjects: WithDeletedObjects);
        }

        public IMongoCollection<TEntity> GetCollection()
        {
            return dbSet;
        }

        public async Task<ReplaceOneResult> Update(FilterDefinition<TEntity> filter, TEntity update)
        {
            var updateRes = await dbSet.ReplaceOneAsync(filter, update);
            return updateRes;
        }

        private IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null, bool WithDeletedObjects = false)
        {
            filter = filter == null ? x => true : filter;
            var collection = dbSet;
            IQueryable<TEntity> query = collection.AsQueryable();
            query = query.Where(filter).ApplyDefaultFilters(WithDeletedObjects);
            return query;
        }

    }
}
