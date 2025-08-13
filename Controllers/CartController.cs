using System.Security.Claims;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace MegaStor.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		private readonly CartRepository _repoCart;
		private readonly IRepository<Product> _repoproduct;
 		private readonly UserManager<ApplicationUser> _userManager;

		public CartController(CartRepository _cart, IRepository<Product> _product 
			,UserManager<ApplicationUser> UserManager)
		{
			_repoCart = _cart;
			_repoproduct = _product;
 			_userManager = UserManager;
		}

		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return Unauthorized();

			if (string.IsNullOrEmpty(user.Id))
				return RedirectToAction("Login", "AccountSettings");

			var findCart = await _repoCart.GetCartByUserIdAsync(user.Id);
			if (findCart == null || findCart.CartItems == null || !findCart.CartItems.Any())
				return View(new CartViewModel()); // عرض كارت فاضي

			CartViewModel cartViewModel = new CartViewModel
			{
				CartId = findCart.CartId,
				TotalAmount = findCart.TotalAmount,
				TotalQuantity = findCart.TotalQuantity,
				CartItems = new List<CartItemViewModel>(),
			};

			foreach (var item in findCart.CartItems)
			{
				var product = _repoproduct.GetAllAsQuery()
					.Include(v => v.Vendor)
					.FirstOrDefault(i => i.Id == item.ProductId); // هنا الأفضل تستخدم ProductId من الكارت

				if (product == null)
					continue; // تخطي لو المنتج مش موجود

				var carItemViewModel = new CartItemViewModel
				{
					CarItemId = item.Id,
					NameItem = product.Name,
					DescriptionProductItem = product.Description,
					PriceItem = item.UnitPrice,
					Quantity = item.Quantity,
					SelerName = product.Vendor.FullName,
					pricediscount = product.DiscountPrice,
					ImageItem = product.ImageUrl,
				};

				cartViewModel.CartItems.Add(carItemViewModel);
			}


			return View(cartViewModel);
		}




		[HttpGet]
		public IActionResult AddToCart()
		{
			return View(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddToCart(int id, int quantity = 1)
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return Unauthorized();

			var product = await _repoproduct.GetByIdAsync(id);

			if (product == null) 
				return NotFound("Product not found");

			// Get or create cart
			var cart = await _repoCart.GetCartByUserIdAsync(user.Id);
 			if (cart == null)
			{
				cart = new Cart
				{
					UserId = user.Id,
					CartItems = new List<CartItem>()
				};
				await _repoCart.AddToCartAsync(cart);
			}

		   

			// Check if product exists in cart
			var existingItem = cart.CartItems
				.FirstOrDefault(i => i.ProductId == id);


			if (existingItem != null)
			{
				existingItem.Quantity += quantity;
				cart.TotalQuantity += quantity;
 				existingItem.TotalPrice = existingItem.Quantity * existingItem.Product.DiscountPrice;
 			}
			else
			{
				var newItem = new CartItem
				{
					ProductId = id,
					CartId = cart.CartId,
					Quantity = quantity,
					UnitPrice = product.DiscountPrice,
					TotalPrice = product.DiscountPrice * quantity
				};

				cart.CartItems.Add(newItem);
			}

			// Update total fields
			cart.TotalQuantity = cart.CartItems.Sum(i => i.Quantity);
			cart.TotalAmount = cart.CartItems.Sum(i => i.TotalPrice);

			await _repoCart.SaveChangesAsync();

			return RedirectToAction("Index", "Home");  
		}




		[HttpPost]
		//[ValidateAntiForgeryToken]//agnore this attribute if you are not using anti-forgery tokens .
		//the library will not work 
		public async Task<IActionResult> DeleteItem(int id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var cart = _repoCart.GetAllCarts(userId)
								.FirstOrDefault(c => c.UserId == userId);

			if (cart == null || cart.CartItems == null)
				return NotFound();

			var item = cart.CartItems.FirstOrDefault(i => i.Id == id);

			if (item == null)
				return NotFound();

			cart.TotalQuantity = Math.Max(0, cart.TotalQuantity - item.Quantity);
			cart.TotalAmount = Math.Max(0, cart.TotalAmount - item.TotalPrice);


			_repoCart.DeleteCartItem(item.Id);
			await _repoCart.SaveChangesAsync();

			return Ok();
		}


		[HttpPost]
		//[ValidateAntiForgeryToken] 
 		public async Task<IActionResult> DeleteCart()
		{
			var user = await _userManager.GetUserAsync(User);
			var carts = _repoCart.GetAllCarts(user.Id).ToList();

			if (!carts.Any())
				return NotFound();

			foreach (var cart in carts)
			{
				if (cart.CartItems != null && cart.CartItems.Any())
				{
					var itemIds = cart.CartItems.Select(ci => ci.Id).ToList();

					foreach (var id in itemIds)
					{
						_repoCart.DeleteCartItem(id);
					}
				}

				cart.TotalAmount = 0;
				cart.TotalQuantity = 0;
			}

			await _repoCart.SaveChangesAsync();
			return Ok();
		}





		[HttpPost]
		public async Task<IActionResult> IncreaseQuantity(int id)
		{
			var CarItem = _repoCart.FindCartItem(id);
			if (CarItem != null)
			{
				CarItem.Quantity++;
				CarItem.Cart.TotalQuantity++;
				CarItem.Cart.TotalAmount+=CarItem.Product.DiscountPrice;
				_repoCart.UpdateCartItem(CarItem);
				await _repoCart.SaveChangesAsync();
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> DecreaseQuantity(int id)
		{
			var CarItem = _repoCart.FindCartItem(id);
			if (CarItem != null && CarItem.Quantity > 1)
			{
				CarItem.Quantity--;
				CarItem.Cart.TotalQuantity--;
				CarItem.Cart.TotalAmount -= CarItem.Product.DiscountPrice;
				_repoCart.UpdateCartItem(CarItem);
				await _repoCart.SaveChangesAsync();
			}
			return RedirectToAction("Index");
		}



	}
}
