using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kobe.Data.Repository
{
    public interface IMongoDBRepository<TEntity>
    {
        IQueryable<TEntity> GetAll(bool WithDeletedObjects = false);
        IQueryable<TEntity> GetById(Expression<Func<TEntity, bool>> whereCondition, bool WithDeletedObjects = false);
        Task<TEntity> Add(TEntity item);
        Task<TEntity> Delete(FilterDefinition<TEntity> item, TEntity model);
        bool Exist(Expression<Func<TEntity, bool>> whereCondition, bool WithDeletedObjects = false);
        Task<ReplaceOneResult> Update(FilterDefinition<TEntity> filter, TEntity model);
    }
}
