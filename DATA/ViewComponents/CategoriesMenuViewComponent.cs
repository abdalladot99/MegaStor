using MegaStor.Models;
using MegaStor.RepositoryFile;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
	public class CategoriesMenuViewComponent:ViewComponent
	{
		private readonly IRepository<Category> _repoCategory;

		public CategoriesMenuViewComponent(IRepository<Category> RepoCategory)
		{
			 _repoCategory = RepoCategory;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{

			var categories = await _repoCategory.GetAllWithIncludeAsync(p=>p.SubCategories);
			return View(categories);
		}

	}
}
