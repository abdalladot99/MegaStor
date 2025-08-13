using MegaStor.DATA;
using MegaStor.Models;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 
namespace MegaStor.Controllers
{
 	public class SearchController : Controller
	{
		private readonly AppDbContext _context;

		public SearchController(AppDbContext context)
		{
			_context = context;
		}
		 

		////Filter And AllProduct     
		[HttpGet]
		public IActionResult Index(string? query,List<string>? categoryIds ,decimal? minPrice, decimal? maxPrice, int page = 1)
		{
 			int pageSize = 12;
			 
			if (string.IsNullOrWhiteSpace(query))
			{
				return View(new AllProductAndFultterViewModel());
			}
			 
			var results = _context.Products
				.Where(p => p.Name.Contains(query) || p.Description.Contains(query)).Include(c => c.SubCategory).AsQueryable();


			// فلترة بالفئة 
			var CategoryIds=new List<string>();
			if (page == 1 || categoryIds.Count==0)
			{ 
				if (categoryIds != null && categoryIds.Any())
					results = results.Where(p => categoryIds.Contains(p.SubCategoryId));
			}
			else
			{
				string OneString = categoryIds[0];
			    CategoryIds = OneString.Split(',').ToList();
				if (CategoryIds != null && CategoryIds.Any())
					results = results.Where(p => CategoryIds.Contains(p.SubCategoryId));
			}
 		
			
			//فلترة بالسعر الأدنى
			if (minPrice.HasValue)
				results = results.Where(p => p.DiscountPrice >= minPrice.Value);

			// فلترة بالسعر الأقصى     
			if (maxPrice.HasValue)
				results = results.Where(p => p.DiscountPrice <= maxPrice.Value);
			 
			var pagedProducts = results
				.OrderBy(p => p.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var Categories = _context.subCategories
				.Select(c => new SubCategory
				{
					SubCategoryId = c.SubCategoryId,
					CategoryName = c.CategoryName,
					Products = c.Products  
				}).ToList();

			AllProductAndFultterViewModel allProductAndFultterViewModel = new AllProductAndFultterViewModel
			{
				Products = pagedProducts,
				SubCategories = Categories,
				query= query,
				maxPrice= maxPrice,
				minPrice= minPrice,
				page= page,
				categoryIds= categoryIds,
			};
			if (page != 1 && categoryIds.Count != 0)
			{
				allProductAndFultterViewModel.categoryIds = CategoryIds;
			}

			ViewBag.counter = results.Count(); 
			ViewBag.pagination = page;

			return View(allProductAndFultterViewModel);
		}
		 


	}

}
