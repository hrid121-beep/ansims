using IMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IMS.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
        public static IQueryable<T> GetQueryable<T>(this IRepository<T> repository) where T : class
        {
            return repository.Query();
        }

        // Extension method for Include with chaining
        public static IQueryable<T> Include<T, TProperty>(
            this IRepository<T> repository,
            Expression<Func<T, TProperty>> navigationPropertyPath) where T : class
        {
            return repository.Query().Include(navigationPropertyPath);
        }

        // Extension method for Where with chaining
        public static IQueryable<T> Where<T>(
            this IRepository<T> repository,
            Expression<Func<T, bool>> predicate) where T : class
        {
            return repository.Query().Where(predicate);
        }

        // Extension method for FirstOrDefault (sync)
        public static T FirstOrDefault<T>(
            this IRepository<T> repository,
            Expression<Func<T, bool>> predicate) where T : class
        {
            return repository.Query().FirstOrDefault(predicate);
        }

        // Extension method for Count (sync)
        public static int Count<T>(
            this IRepository<T> repository,
            Expression<Func<T, bool>> predicate = null) where T : class
        {
            var query = repository.Query();
            return predicate == null ? query.Count() : query.Count(predicate);
        }

        // Extension method for UpdateAsync
        public static async Task UpdateAsync<T>(
            this IRepository<T> repository,
            T entity) where T : class
        {
            repository.Update(entity);
            await Task.CompletedTask;
        }
    }
}