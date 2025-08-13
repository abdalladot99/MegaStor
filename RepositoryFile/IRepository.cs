using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MegaStor.RepositoryFile
{
    public interface IRepository<T>where T : class
    {
    
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task<bool> DeleteAsync(int id);
        Task SaveAsync();



        Task<IEnumerable<T>> GetAllWithIncludeAsync(Expression<Func<T, object>> includeExpression);

        Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includeExpressions);

        IQueryable<T> GetAllAsQuery();


	}
}
