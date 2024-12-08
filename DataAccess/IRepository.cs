using System.Linq.Expressions;

namespace DataAccess;

public interface IRepository<T> where T : class
{
    IQueryable<T> Get(Expression<Func<T, bool>>? filter);
    T Insert(T entity);
}