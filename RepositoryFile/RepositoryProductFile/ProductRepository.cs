using MegaStor.DATA;
using MegaStor.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace MegaStor.RepositoryFile.RepositoryProductFile
{
	public class ProductRepository
	{
		private readonly AppDbContext _dbContext;

		public ProductRepository(AppDbContext DbContext)
		{
			_dbContext = DbContext;
		}

		public async Task<Product> GetProdById(int id)
		{
			return await _dbContext.Products
				.Include(p => p.Category)
				.Include(p => p.SubCategory)
				.Include(p => p.Reviews)
				     .ThenInclude(c=>c.Customer)
 				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<Review?> GetExistingReviewAsync(int productId, string userId)
		{
			return await _dbContext.Reviews
				.FirstOrDefaultAsync(r => r.ProductId == productId && r.CustomerId == userId);
		}


		public IQueryable<Product> GetAllProducts()
		{
			return _dbContext.Products;
		}





	}
}
