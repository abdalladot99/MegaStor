using System.Linq.Expressions;
using MegaStor.DATA;
using MegaStor.Models;
using Microsoft.EntityFrameworkCore;
namespace MegaStor.RepositoryFile.RepositoryProductFile
{
 
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
  

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


 
        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(Expression<Func<T, object>> includeExpression)
        {
            return await _context.Set<T>().Include(includeExpression).ToListAsync();
        }


		public async Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includeExpressions)
		{
			IQueryable<T> query = _context.Set<T>();

			foreach (var includeExpression in includeExpressions)
			{
				query = query.Include(includeExpression);
			}

			return await query.ToListAsync();
		}

         
		public IQueryable<T> GetAllAsQuery()
		{
			return _context.Set<T>();
		}


	}

}
