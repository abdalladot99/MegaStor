using MegaStor.Models;
using MegaStor.RepositoryFile;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
	public class CategorieSectionViewComponent : ViewComponent
	{
		private readonly IRepository<Category> cate;
		public CategorieSectionViewComponent(IRepository<Category> Cate)
		{
			 cate = Cate;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			IEnumerable<Category> list = await cate.GetAllWithIncludeAsync(s => s.SubCategories);
			ViewBag.counter = list.Count();
			return View(list);
			 
		}
	}
}
