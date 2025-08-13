using System.Security.Claims;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel; 
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
    public class CarInHomeMenuViewComponent : ViewComponent
    {
        private readonly CartRepository _repoCart;
        private readonly IRepository<Product> prod;
         private readonly IHttpContextAccessor httpContextAccessor;

        public CarInHomeMenuViewComponent (CartRepository RepoCart, IRepository<Product> _product,
             IHttpContextAccessor _httpContextAccessor)
        {
			_repoCart = RepoCart;
            prod = _product;
            httpContextAccessor = _httpContextAccessor;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
 			var customerId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);  
			 

			if (string.IsNullOrEmpty(customerId))
			{
				return View( "EmptyCart");
			}

			var findCart = await _repoCart.GetCartByUserIdAsync(customerId);

			if (findCart == null)
			{
				return View("EmptyCart");
			}
			CartViewModel cartViewModel = new CartViewModel
			{
				CartId = findCart.CartId,
				TotalAmount = findCart.TotalAmount,
				TotalQuantity = findCart.TotalQuantity,
				CartItems = new List<CartItemViewModel>(),
			};


			foreach (var item in findCart.CartItems)
			{
				var product = await prod.GetByIdAsync(item.ProductId);
				if (product == null)
					continue; // Skip if product not found
				var carItemViewModel = new CartItemViewModel
				{
					CarItemId = product.Id,
					NameItem = item.Product.Name,
					DescriptionProductItem = item.Product.Description,
					PriceItem = item.UnitPrice,
					Quantity = item.Quantity,
 					pricediscount = item.Product.DiscountPrice,
					ImageItem = item.Product.ImageUrl,
					//SelerName = item.Product.Vendor.UserName,

				};

				cartViewModel.CartItems.Add(carItemViewModel);
			}

			return View(cartViewModel);

		}










	}
}