using System.Diagnostics;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegaStor.Controllers
{
	[Authorize]
    public class HomeController : Controller
    {
 
		private readonly IRepository<Category> _repoCategory;
		private readonly IRepository<Product> _repoProduct;
		private readonly IRepository<Review> _repoReviewProduct;
		private readonly IRepository<SubCategory> _repoSubCategory;
		private readonly ProductRepository _repoProductCost;


		public HomeController( IRepository<Category> RepoCategory, IRepository<Product> RepoProduct, IRepository<SubCategory> SubCategory
			, ProductRepository RepoProductCost, IRepository<Review> RepoReviewProduct)
        {
			_repoCategory = RepoCategory;
			_repoProduct = RepoProduct;
			_repoSubCategory = SubCategory;
			_repoProductCost = RepoProductCost;
			_repoReviewProduct = RepoReviewProduct;
		}
		 
		public async Task<IActionResult> Index(int page = 1)
		{
			const int pageSize = 12;

			var categories = await _repoCategory.GetAllAsync();
			var subCategories = await _repoSubCategory.GetAllAsync();

			// ??? ??? ???????? ??? ?????? pagination
			int totalProducts = await _repoProductCost.GetAllProducts().CountAsync();

			// ??? ???? ???????? ?? ???????? ????????
			var pagedProducts = await _repoProductCost.GetAllProducts()
				.Include(p => p.Category)
				.Include(p => p.SubCategory)
				.Include(p => p.Reviews)
				.OrderBy(p => p.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// ?????? ????? (???? ??? 7 ????)
			DateTime oneWeekAgo = DateTime.UtcNow.AddDays(-7);
			var newProducts = await _repoProduct.GetAllAsQuery()
				.Where(p => p.CreatedAt >= oneWeekAgo)
				.OrderByDescending(p => p.CreatedAt)
				.ToListAsync();

			// ???? ???????? ??????? - ???? ??? ???? 10 ??????
			var topRatedReviews = await _repoReviewProduct.GetAllAsQuery()
				.Include(r => r.Product)
				.OrderByDescending(r => r.Rating)
				.Take(10)
				.ToListAsync();

			var topRatedProducts = topRatedReviews
				.Select(r => r.Product)
				.Distinct()
				.ToList();

			// ?????? ??? ??? - ???? ???? 10 ??????
			var topDiscountProducts = await _repoProduct.GetAllAsQuery()
				.Where(p => p.discount > 0)
				.OrderByDescending(p => p.discount)
				.Take(10)
				.ToListAsync();

			// ????? ViewModel
			var model = new HomeIndexAllEverythingViewModel
			{
				categories = categories,
				subCategories = subCategories,
				AllproductsBySkip = pagedProducts,
				Newproducts = newProducts,
				TopSellingProducts = topRatedProducts,
				TopDiscountProducts = topDiscountProducts
			};

			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

			return View(model);
		}

		 
		public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




    }
}
