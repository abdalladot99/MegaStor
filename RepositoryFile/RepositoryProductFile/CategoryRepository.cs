using MegaStor.DATA;
using MegaStor.Models;
using Microsoft.EntityFrameworkCore;

namespace MegaStor.RepositoryFile.RepositoryProductFile
{
	public class CategoryRepository
	{
		private readonly AppDbContext _context;

		public CategoryRepository(AppDbContext context)
		{
			_context = context;
		}
		public Task<Category?> GetByIdWithProductsAsync(int id) =>
		_context.Categories
		.Include(c => c.Products)
		.Include(c => c.SubCategories)
   		.FirstOrDefaultAsync(c => c.Id == id);

		public Task<SubCategory?> GetByIdWithProductsforSubAsync(string id) =>
		_context.subCategories
		.Include(c => c.Products)
		.Include(c => c.Category)
   		.FirstOrDefaultAsync(c => c.SubCategoryId==id);


		public async Task<bool> DeleteSubAsync(string id)
		{
			var entity = await GetByIdWithProductsforSubAsync(id);
			if (entity != null)
			{
				_context.Set<SubCategory>().Remove(entity);
				_context.SaveChanges();
				return true;
			}
			return false;
		}
	}
}
