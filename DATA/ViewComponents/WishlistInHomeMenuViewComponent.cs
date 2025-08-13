using System.Security.Claims;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
	public class WishlistInHomeMenuViewComponent : ViewComponent
	{
		private readonly IRepository<Wishlists> Wishlist;
		private readonly IRepository<Product> prod;
 		private readonly IHttpContextAccessor httpContextAccessor;

		public WishlistInHomeMenuViewComponent(IRepository<Wishlists> _Wishlist, IRepository<Product> _product,
			  IHttpContextAccessor _httpContextAccessor)
		{
			Wishlist = _Wishlist;
			prod = _product;
 			httpContextAccessor = _httpContextAccessor;
		}


		public async Task<IViewComponentResult> InvokeAsync()
		{

			var customerId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); // هذا هو الـ UserId
 
			var cartItems = Wishlist.GetAllAsync().Result
				.Where(ci => ci.CustomerId == customerId)
				.ToList();

			List<WishlistViewModel> wishlistList = new List<WishlistViewModel>();
			foreach (var item in cartItems)
			{
				var product = await prod.GetByIdAsync(item.ProductId);

				if (product == null)
				{
					return View();
				}

				WishlistViewModel wishlistViewModel = new WishlistViewModel();
				wishlistViewModel.Id = product.Id;
				wishlistViewModel.Name = product.Name;
				wishlistViewModel.Description = product.Description;
				wishlistViewModel.Price = product.Price;
				wishlistViewModel.pricediscount = product.DiscountPrice;
				wishlistViewModel.ImageUrl = product.ImageUrl;
 

				wishlistList.Add(wishlistViewModel);

			}

			return View(wishlistList);

		}


	}
}
