using MegaStor.Constants.Enum;
using MegaStor.DATA;
using MegaStor.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace MegaStor.Services
{
	public class OrderServices
	{

		private readonly ISenderEmail _senderEmail;
		private readonly AppDbContext _context;

		public OrderServices(ISenderEmail SenderEmail,AppDbContext DbContext)
		{
 			_senderEmail = SenderEmail;
			_context = DbContext;
		}


		public async Task SendOrderConfirmationEmail(string toEmail, string orderId, string orderAmount)
		{
			string subject = $"Order Confirmation - Order #{orderId}";
			string body = $@"
			<h1>Thank you for your order!</h1>
			<p>Your order ID is <strong>{orderId}</strong>.</p>
			<p>Order Amount:</p>
			<p>Your order total amount is {orderAmount}</p>
			<p>We will notify you once your order is shipped.</p>";

			await _senderEmail.SendEmailAsync(toEmail, subject, body, true);
		}

		// send email when order status changes
		public async Task SendOrderStatusUpdateEmail(string toEmail, string orderId, string newStatus)
		{
			string unDeliveredOrder = "";

			if (newStatus != OrderShippingStatusEnum.Delivered.ToString().ToLower())
			{
				unDeliveredOrder = "Soon it will be delivered";
			}

			string subject = $"Order Status Update - Order #{orderId}";
			string body = $@"
        <h1>Order Status Updated</h1>
        <p>Your order with ID <strong>{orderId}</strong> has been {newStatus}.</p>
        <p>{unDeliveredOrder}</p>
        <p>Thank you for shopping with us!</p>";

			await _senderEmail.SendEmailAsync(toEmail, subject, body, true);
		}




		// Get Order by UserId
		public async Task<List<OrderViewModel>> GetOrderByUserAsync(string id)
		{
			var orders = await _context.Orders
							.Where(o => o.CustomerId == id)
							.Include(o => o.OrderItems)
							.ThenInclude(oi => oi.Product)
							.Include(o => o.ShippingAddress)
							.Include(o => o.BillingAddress)
							.Select(o => new OrderViewModel
							{
								OrderId = o.Id,
								OrderDate = o.OrderDate,
								OrderStatus = o.OrderStatus,
								ShippingStatus = o.ShippingStatus,
								TotalQuantity = o.TotalQuantity,
								TotalAmount = o.TotalAmount,
								shippingAddress = new ShippingAddressViewModel
								{
									FirstName = o.ShippingAddress.FirstName,
									LastName = o.ShippingAddress.LastName,
									Address = o.ShippingAddress.Address,
									City = o.ShippingAddress.City,
									Country = o.ShippingAddress.Country,
									PhoneNumber = o.ShippingAddress.PhoneNumber,
									PostalOrZipCode = o.ShippingAddress.PostalOrZipCode,
								},
								billingAddress = new BillingAddressViewModel
								{
									FirstName = o.BillingAddress.FirstName,
									LastName = o.BillingAddress.LastName,
									Address = o.BillingAddress.Address,
									City = o.BillingAddress.City,
									Country = o.BillingAddress.Country,
									PhoneNumber = o.BillingAddress.PhoneNumber,
									PostalOrZipCode = o.BillingAddress.PostalOrZipCode,
								},
								OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
								{
									ProductName = oi.Product.Name,
									UnitPrice = Convert.ToDecimal(oi.UnitPrice),
									Quantity = oi.Quantity,
									TotolPrice = Convert.ToDecimal(oi.TotalPrice),
								}).ToList()

							}).ToListAsync();

			return orders;
		}


		////////////////////////



		public List<decimal> MonthlySales()
		{
			var sales = new List<decimal>();

			// جلب قائمة الشهور اللي حسبناها
			var months = MonthLabels();
			var currentYear = DateTime.Now.Year;
			var currentMonth = DateTime.Now.Month;

			// تحديد أول شهر وأول سنة من القائمة
			var startMonth = currentMonth - 4;
			var startYear = currentYear;

			if (startMonth <= 0)
			{
				startMonth += 12;
				startYear--;
			}

			for (int i = 0; i < 9; i++) // 4 قبل + الحالي + 4 بعد
			{
				var monthDate = new DateTime(startYear, startMonth, 1);
				var nextMonthDate = monthDate.AddMonths(1);

				// مجموع المبيعات في الشهر ده
				var total = _context.Orders
					.Where(o => o.OrderDate >= monthDate && o.OrderDate < nextMonthDate)
					.Sum(o => (decimal?)o.TotalAmount ?? 0);

				sales.Add(total);

				// الانتقال للشهر اللي بعده
				startMonth++;
				if (startMonth > 12)
				{
					startMonth = 1;
					startYear++;
				}
			}

			return sales;
		}
		public List<int> MonthlyOrders()
		{
			var ordersCount = new List<int>();

			var currentMonth = DateTime.Now.Month;
			var currentYear = DateTime.Now.Year;

			var startMonth = currentMonth - 4;
			var startYear = currentYear;

			if (startMonth <= 0)
			{
				startMonth += 12;
				startYear--;
			}

			for (int i = 0; i < 9; i++) // 4 قبل + الحالي + 4 بعد
			{
				var monthDate = new DateTime(startYear, startMonth, 1);
				var nextMonthDate = monthDate.AddMonths(1);

				// عدد الطلبات في الشهر ده
				var count = _context.Orders
					.Where(o => o.OrderDate >= monthDate && o.OrderDate < nextMonthDate)
					.Count();

				ordersCount.Add(count);

				// انتقال للشهر اللي بعده
				startMonth++;
				if (startMonth > 12)
				{
					startMonth = 1;
					startYear++;
				}
			}

			return ordersCount;
		}
		public List<string> MonthLabels()
		{
			var currentMonth = DateTime.Now.Month;
			var currentYear = DateTime.Now.Year;

			// نبدأ من 4 شهور قبل الشهر الحالي
			var startMonth = currentMonth - 4;
			var startYear = currentYear;

			if (startMonth <= 0)
			{
				startMonth += 12;
				startYear--;
			}

			var monthLabels = new List<string>();

			for (int i = 0; i < 9; i++) // 4 قبل + الحالي + 4 بعد
			{
				var monthDate = new DateTime(startYear, startMonth, 1);
				monthLabels.Add(monthDate.ToString("MMM"));

				startMonth++;
				if (startMonth > 12)
				{
					startMonth = 1;
					startYear++;
				}
			}

			return monthLabels;
		}







	}
}
