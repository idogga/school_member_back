using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

public static class DBContextExtensions
{
        public static async Task<T> FirstOrFail<T>(
            this DbSet<T> query,
            Expression<Func<T, bool>> predicate)
            where T : class
        {
            var result = await query.FirstOrDefaultAsync(predicate);

            if (result is null)
            {
                throw new NotFoundException(typeof(T));
            }

            return result;
        }

        public static async Task<T> FirstOrFail<T, TProperty>(
            this IIncludableQueryable<T, TProperty> query,
            Expression<Func<T, bool>> predicate)
            where T : class
        {
            var result = await query.FirstOrDefaultAsync(predicate);

            if (result is null)
            {
                throw new NotFoundException(typeof(T));
            }

            return result;
        }

        public static async Task<T> FirstOrFail<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate)
            where T : class
        {
            var result = await query.FirstOrDefaultAsync(predicate);

            if (result is null)
            {
                throw new NotFoundException(typeof(T));
            }

            return result;
        }

        public static async Task<T> FirstOrFail<T>(
            this IQueryable<T> query)
            where T : class
        {
            var result = await query.FirstOrDefaultAsync();

            if (result is null)
            {
                throw new NotFoundException(typeof(T));
            }

            return result;
        }

        public static T FirstOrFail<T>(
            this IEnumerable<T> query,
            Func<T, bool> predicate)
            where T : class
        {
            var result = query.FirstOrDefault(predicate);

            if (result is null)
            {
                throw new NotFoundException(typeof(T));
            }

            return result;
        }

        public static async Task<T> FindOrFailAsync<T>(
            this DbSet<T> query,
            params object[] keyValues)
            where T : class
        {
            var result = await query.FindAsync(keyValues);

            if (result is null)
            {
                throw new NotFoundException(typeof(T));
            }

            return result;
        }
}