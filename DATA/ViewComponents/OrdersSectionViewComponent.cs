using MegaStor.RepositoryFile.RepositoryProductFile;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
	public class OrdersSectionViewComponent : ViewComponent
	{
		private readonly OrderRepository _orderRepo;

		public OrdersSectionViewComponent(OrderRepository OrderRepo)
		{
			_orderRepo = OrderRepo;
		}
	 
         
		public async Task<IViewComponentResult> InvokeAsync()
		{
			ViewData["ActiveParent"] = "Orders";
			ViewData["ActiveLink"] = "AllOrders";
			var Orders = await _orderRepo.GetAllWithIncludeAsync(o => o.Customer,o => o.OrderItems, o => o.ShippingAddress,
				o => o.BillingAddress, o => o.Payment);
			Orders = Orders.OrderByDescending(o => o.OrderDate).ToList();

			return View(Orders);
		}
	}
}
