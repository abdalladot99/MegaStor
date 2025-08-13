using MegaStor.Constants.Enum;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.Services;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace MegaStor.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly CartRepository _cartRepo;
		private readonly OrderRepository _orderrepo;
		private readonly PaymobService _paymobService;
		private readonly OrderServices _orderServices;
		private readonly IRepository<Product> _repoProduct;
 		public OrderController(UserManager<ApplicationUser> userManager, CartRepository Cartrepo, OrderRepository Orderrepo,
			 PaymobService PaymobService,IRepository<Product> RepoProduct , OrderServices OrderServices)
		{
			_userManager = userManager;
			_cartRepo = Cartrepo;
			_orderrepo = Orderrepo;
			_paymobService = PaymobService;
			_repoProduct = RepoProduct;
			_orderServices = OrderServices;
		}



		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index()
		{
			ViewData["ActiveParent"] = "Orders";
			ViewData["ActiveLink"] = "AllOrders";
			var Orders = await _orderrepo.GetAllWithIncludeAsync(o => o.Customer, o => o.OrderItems, o => o.ShippingAddress,
				o => o.BillingAddress, o => o.Payment);
			Orders = Orders.OrderByDescending(o => o.OrderDate).ToList();

			return View(Orders);
		}



		[HttpGet]
		public async Task<IActionResult> Details(string orderId)
		{
			ViewData["ActiveParent"] = "Orders";

			var order = await _orderrepo.GetOrderByOrderIdAsync(orderId);
			//After the test will be delete 
			order.Payment = new Payment();
			order.BillingAddress = new BillingAddress();
			order.ShippingAddress = new ShippingAddress();

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}



		private async Task<Order> ProcessOrderAsync(CheckOutViewModel model)
		{

			var userId = _userManager.GetUserId(User);

			var cartUser = await _cartRepo.GetCartByUserIdAsync(userId);


			var shippingAddress = new ShippingAddress
			{
				FirstName = model.ShippingAddress.FirstName,
				LastName = model.ShippingAddress.LastName,
				Country = model.ShippingAddress.Country,
				PhoneNumber = model.ShippingAddress.PhoneNumber,
				EmailAddress = model.ShippingAddress.EmailAddress,
				Address = model.ShippingAddress.Address,
				StateOrProvince = model.ShippingAddress.StateOrProvince,
				City = model.ShippingAddress.City,
				PostalOrZipCode = model.ShippingAddress.PostalOrZipCode,
			};

			if (model.ShippingAddress.Address==null)
			{
				shippingAddress = new ShippingAddress
				{
					FirstName = model.BillingAddress.FirstName,
					LastName = model.BillingAddress.LastName,
					Country = model.BillingAddress.Country,
					PhoneNumber = model.BillingAddress.PhoneNumber,
					EmailAddress = model.BillingAddress.EmailAddress,
					Address = model.BillingAddress.Address,
					StateOrProvince = model.BillingAddress.StateOrProvince,
					City = model.BillingAddress.City,
					PostalOrZipCode = model.BillingAddress.PostalOrZipCode,
				};

			}
			var billingAddress = new BillingAddress
			{
				FirstName = model.BillingAddress.FirstName,
				Country = model.BillingAddress.Country,
				PhoneNumber = model.BillingAddress.PhoneNumber,
				EmailAddress = model.BillingAddress.EmailAddress,
				Address = model.BillingAddress.Address,
				StateOrProvince = model.BillingAddress.StateOrProvince,
				City = model.BillingAddress.City,
				PostalOrZipCode = model.BillingAddress.PostalOrZipCode,
			};

			var order = new Order
			{
				CustomerId = userId,
				OrderDate = DateTime.Now,
				ShippingAddress = shippingAddress,
				BillingAddress = billingAddress,
				OrderStatus = OrderPlacementStatusEnum.Pending,
				ShippingStatus = OrderShippingStatusEnum.Pending,
				TotalQuantity = cartUser.TotalQuantity,
				TotalAmount = cartUser.TotalAmount,
				OrderItems = new List<OrderItem>(),
			};

			foreach (var cartItems in cartUser.CartItems)
			{
				var orderItem = new OrderItem
				{
					OrderId = order.Id,
					ProductId = cartItems.ProductId,
					Quantity = cartItems.Quantity,
					UnitPrice = cartItems.Product.DiscountPrice,
					TotalPrice = cartItems.Quantity * cartItems.Product.DiscountPrice,
				};
				order.OrderItems.Add(orderItem);
			}
			order.Payment = new Payment
			{
				OrderId = order.Id,
				UserId = userId,
				Amount = order.TotalAmount,
				PaymentDate = DateTime.Now,
				PaymentMethod = model.SelectedPaymentMethod,
				PaymentStatus = "Pending",
			};

			await _orderrepo.AddAsync(order);
 
			await _orderrepo.SaveAsync();


			return order;

		}



		[HttpGet]
		public async Task<IActionResult> Checkout()
		{

			var userId = _userManager.GetUserId(User);

			var cartUser = await _cartRepo.GetCartByUserIdAsync(userId);

			if (cartUser == null)
			{
				return View(new CheckOutViewModel
				{
					CartItems = new List<CartItemViewModel>(),
				});
			}

			CheckOutViewModel viewModel = new CheckOutViewModel
			{
				CartItems = cartUser.CartItems
				.Select(ci => new CartItemViewModel
				{
					CarItemId = ci.Id,
					NameItem = ci.Product.Name,
					Quantity = ci.Quantity,
					TotalPricForItems = ci.TotalPrice,
					pricediscount = ci.Product.DiscountPrice,
					DescriptionProductItem = ci.Product.Description,
					PriceItem = ci.Product.Price,
					ImageItem = ci.Product.ImageUrl,

				}).ToList(),

			};

			viewModel.TotalAmount = cartUser.CartItems.Sum(i => i.Quantity * i.Product.DiscountPrice);
			
			return View(viewModel);

 		}
		 

		[HttpPost]
		public async Task<IActionResult> PlaceOrder(CheckOutViewModel model)
		{


			if (!ModelState.IsValid)
			{
				return View("Checkout", model);
			}

			// إنشاء الطلب (لكن لسه مش بنخزنه)
			var order = await ProcessOrderAsync(model);

			if (model.SelectedPaymentMethod == PaymentMethodsEnum.CreditOrDebitCard)
			{
				// استدعاء PaymobService علشان تجيب لينك الدفع
				var paymentUrl = await _paymobService.StartPaymentAsync(
					totalAmount: order.TotalAmount,
					email: model.BillingAddress.EmailAddress,
					phone: model.BillingAddress.PhoneNumber,
					name: model.BillingAddress.FirstName
				);

				// حفظ الطلب مؤقتًا في TempData أو Session لو حابب، أو تنتظر Webhook بعد الدفع
				TempData["PendingOrderId"] = order.Id;

				// تحويل المستخدم لصفحة Paymob
				return Redirect(paymentUrl);
			}

			return RedirectToAction("OrderConfirmation", new { id = order.Id });
		}


		[AllowAnonymous]
		public IActionResult PaymentResult(bool success, string message)
		{
			ViewBag.Success = success;
			ViewBag.Message = string.IsNullOrEmpty(message)
				? (success ? "Payment Successful ✅" : "Payment Failed ❌")
				: message;

			return View();
		}






		[HttpGet]
		public async Task<IActionResult> OrderConfirmation(String id)
		{

			var order = await _orderrepo.GetOrderByOrderIdAsync(id);

			if (order == null)
			{
				return NotFound("Order not found.");
			}

			return View(order);
		}



		//Complete order
	    [HttpPost]
		public async Task<IActionResult> CompleteOrder(string orderId)
		{
			var order = await _orderrepo.GetOrderByOrderIdAsync(orderId);

			var user = await _userManager.GetUserAsync(User);

			var cartUser = await _cartRepo.GetCartByUserIdAsync(user.Id);

			foreach (var cartItem in cartUser.CartItems)
			{
				_cartRepo.DeleteCartItem(cartItem.Id);
			}
			cartUser.TotalAmount = 0;
			cartUser.TotalQuantity = 0;


			if (order == null)
			{
				return NotFound();
			}

			if (order.OrderStatus == OrderPlacementStatusEnum.Completed)
			{
				return RedirectToAction("OrderSummary", new { orderId = order.Id });
			}


			foreach (var item in order.OrderItems)
			{
				var prodcut = await _repoProduct.GetByIdAsync(item.ProductId);
				if (prodcut.IsAvailable)
				{
					prodcut.Quantity -= item.Quantity;
				}
				prodcut.IsAvailable= prodcut.Quantity > 0 ? true : false ;

				_repoProduct.Update(prodcut);

			}

			if (order.Payment.PaymentMethod == PaymentMethodsEnum.CreditOrDebitCard)
			{
				order.Payment.PaymentStatus = "Success";
			}

			string orderAmount = order.TotalAmount.ToString() + "$";
			order.OrderStatus = OrderPlacementStatusEnum.Completed;
			order.OrderDate = DateTime.Now;
			  
  			await _orderServices.SendOrderConfirmationEmail(user.Email, order.Id, orderAmount);
			 
			await _repoProduct.SaveAsync();

			return RedirectToAction("OrderSummary", new { orderId = order.Id });
		}


		// GET Successful Order View
		public async Task<IActionResult> OrderSummary(string orderId)
		{
			var order = await _orderrepo.GetOrderByOrderIdAsync(orderId);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}



		public async Task<IActionResult> DownloadInvoice(string orderId)
		{
			var order = await _orderrepo.GetOrderByOrderIdAsync(orderId);

			var invoice = new InvoicePdfModel
			{
				CustomerName = order.BillingAddress.FirstName +" "+ order.BillingAddress.LastName,
				Address = order.BillingAddress.Address,
				Email = order.BillingAddress.EmailAddress,
				PhoneNumber = order.BillingAddress.PhoneNumber,
				OrderDate = order.OrderDate.ToString("dd MMM yyyy"),
				OrderStatus = order.OrderStatus.ToString(),
				ShippingStatus = order.ShippingStatus.ToString(),
				TotalAmount = "$" + order.TotalAmount,
				Items = order.OrderItems.Select(item => new InvoiceItemModel
				{
					ItemName = item.Product.Name,
					Quantity = item.Quantity.ToString(),
					UnitPrice = "$" + item.UnitPrice.ToString("0.00"),
					TotalPrice = "$" + item.UnitPrice.ToString("0.00")
				}).ToList()
			};

			var pdfBytes = InvoicePdfGeneratorServices.Generate(invoice);

			return File(pdfBytes, "application/pdf", "Invoice.pdf");
		}



		// Cancel Order
		[HttpPost]
		public async Task<IActionResult> CancelOrder(string orderId)
		{
			var order = await _orderrepo.GetById(orderId);

			if (order == null)
			{
				return NotFound();
			}

			order.OrderStatus = OrderPlacementStatusEnum.Cancelled;
			order.OrderDate = DateTime.Now;
			order.ShippingStatus = OrderShippingStatusEnum.Cancelled;

			await _orderrepo.SaveAsync();

			return RedirectToAction("OrderCancellation", new { orderId = order.Id });
		}




		// GET Cancel Order View
		[HttpGet]
		public IActionResult OrderCancellation(string orderId)
		{
			return View();
		}




		// Display User's Orders
		public async Task<IActionResult> GetMyOrders()
		{
			var user = await _userManager.GetUserAsync(User);

			var orders = await _orderServices.GetOrderByUserAsync(user.Id);

			orders = orders.OrderByDescending(o => o.OrderDate).ToList();
			ViewBag.Action = "My Orders";
			return View(orders);
		}


		// Get order details
		public async Task<IActionResult> MyOrderDetails(string orderId)
		{
			var order = await _orderrepo.GetOrderByOrderIdAsync(orderId);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}



	}
}
