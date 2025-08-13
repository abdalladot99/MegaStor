using MegaStor.Models;
using MegaStor.RepositoryFile;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
 	public class ProductsSectionViewComponent : ViewComponent
	{
		private readonly IRepository<Product> repoProduct;
 		public ProductsSectionViewComponent(IRepository<Product> RepoProduct)
		{
			repoProduct = RepoProduct;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var listPorducts =await repoProduct.GetAllWithIncludeAsync(c=>c.Category,v=>v.Vendor,r=>r.Reviews);
			return View(listPorducts);
		}

	}
}
