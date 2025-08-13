using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MegaStor.Controllers
{
	[Route("PaymobCallback/HandleCallback")]
	[AllowAnonymous]
	public class PaymobCallbackController : Controller
	{
		private readonly OrderRepository _orderRepo;

		public PaymobCallbackController(OrderRepository orderRepo)
		{
			_orderRepo = orderRepo;
		}

		[HttpPost]
		public async Task<IActionResult> HandleCallback([FromBody] PaymobCallbackViewModel payload)
		{
			// استخرج ID من الطلب المحفوظ مؤقتًا ← ممكن يكون في ExternalReference أو TempData أو أي شيء آخر
			var OrderId = payload.obj.order.id.ToString();

			if (payload?.obj?.success == true)
			{ 
				var order = await _orderRepo.GetById(OrderId);
				if (order != null)
				{
					order.Payment.PaymentStatus = "Paid";
					order.Payment.PaymentDate = DateTime.Now;

					order.OrderStatus = Constants.Enum.OrderPlacementStatusEnum.Confirmed;

					await _orderRepo.SaveAsync();
				}
			}

			return RedirectToAction("CompleteOrder", "Order", new {orderId = OrderId });
		}
	}
}
