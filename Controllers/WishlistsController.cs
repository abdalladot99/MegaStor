using System.Security.Claims;
using MegaStor.AddImge;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
namespace MegaStor.Controllers
{
	[Authorize]
	public class WishlistsController : Controller
	{
		 
			private readonly IRepository<Wishlists> wish;
			private readonly IRepository<Product> prod;
			private readonly RepositoryForUser user;
			private readonly AddAndDeleteImageInServer saveImage;

			public WishlistsController(IRepository<Wishlists> _caritem, IRepository<Product> _product, RepositoryForUser user,

				AddAndDeleteImageInServer SaveImage)
			{
				wish = _caritem;
				prod = _product;
				this.user = user;
				saveImage = SaveImage;
			}

			public async Task<IActionResult> Index()
			{
				var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier); // هذا هو الـ UserId

				if (string.IsNullOrEmpty(customerId))
				{
					return RedirectToAction("Login", "AccountSettings"); // إعادة التوجيه إلى صفحة تسجيل الدخول إذا لم يكن المستخدم مسجلاً الدخول
				}

				var cartItems = wish.GetAllAsync().Result
					.Where(ci => ci.CustomerId == customerId)
					.ToList();
			 
			List<WishlistViewModel> carviewmodel = new List<WishlistViewModel>();
				foreach (var item in cartItems)
				{
					var product = await prod.GetByIdAsync(item.ProductId);

					if (product == null)
					{
						return NoContent();
					}

					WishlistViewModel carViewModel = new WishlistViewModel();
					carViewModel.Id = product.Id;
					carViewModel.Name = product.Name;
					carViewModel.Description = product.Description;
					carViewModel.Price = product.Price;
					carViewModel.pricediscount = product.DiscountPrice;
					carViewModel.ImageUrl = product.ImageUrl;
			     	carViewModel.DateAdded = item.DateAdded;


				var vendor = await user.GetUserByIdAsync(product.VendorId);

					carViewModel.SelerName = vendor.UserName;


					carviewmodel.Add(carViewModel);

				}

				return View(carviewmodel);

			}





			[HttpGet]
			public IActionResult AddWishList()
			{
				return View(nameof(Index));
			}

			[HttpPost]
			[ValidateAntiForgeryToken]
			public async Task<IActionResult> AddWishList(int id)
			{
				var product = await prod.GetByIdAsync(id);
				if (product == null)
				{
					return NotFound();
				}

				var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier); // هذا هو الـ UserId
				if (customerId == null)
				{
					return RedirectToAction("Login", "AccountSettings");
				}
				Wishlists cartItem = new Wishlists
				{
					ProductId = product.Id,
					CustomerId = customerId
				};

				await wish.AddAsync(cartItem);
				await wish.SaveAsync();

				return RedirectToAction("Index", "Home");
			}




			[HttpPost]
			//[ValidateAntiForgeryToken]//agnore this attribute if you are not using anti-forgery tokens .
			//the library will not work 
			public async Task<IActionResult> Delete(int Id)
			{
				var listprod = await wish.GetAllAsync();
				Wishlists one = new Wishlists();
				foreach (var item in listprod)
				{
					if (item.ProductId == Id)
					{
						one = item;
						break;
					}
				}
				var result = await wish.DeleteAsync(one.Id);
				if (result)
				{

					return Ok();
				}
				return View(NotFound());

			}


			[HttpPost]
			//[ValidateAntiForgeryToken]
			public async Task<IActionResult> DeleteAll(int i)
			{
				var allItems = await wish.GetAllAsync(); // هات كل العناصر

				foreach (var item in allItems)
				{
					var uesult = await wish.DeleteAsync(item.Id); // حذف العنصر
				}
				return Ok();
		 
			}




		
	}
}