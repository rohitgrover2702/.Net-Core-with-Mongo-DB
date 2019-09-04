using Kobe.Domain.Collections;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kobe.Data.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> ApplyDefaultFilters<TEntity>(this IQueryable<TEntity> query, bool WithDeletedObjects)
        {
            var defaultFilters = GetDefaultFilters<TEntity>(WithDeletedObjects);
            return query.Where(defaultFilters);
        }
        
        //insert all default filters here.
        private static ExpressionStarter<TEntity> GetDefaultFilters<TEntity>(bool WithDeletedObjects)
        {
            var predicate = PredicateBuilder.New<TEntity>(x => true);
            if (typeof(BaseCollection).IsAssignableFrom(typeof(TEntity)) && !WithDeletedObjects)
            {
                predicate.And(x => ((BaseCollection)x).IsDeleted != true);
            }
            return predicate;
        }
    }
}
