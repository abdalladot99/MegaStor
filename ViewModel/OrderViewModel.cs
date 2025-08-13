using MegaStor.Constants.Enum;

namespace MegaStor.ViewModel
{
	public class OrderViewModel
	{
		public string OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public OrderPlacementStatusEnum OrderStatus { get; set; }
		public OrderShippingStatusEnum ShippingStatus { get; set; }
		public ShippingAddressViewModel shippingAddress { get; set; }
		public BillingAddressViewModel billingAddress { get; set; }

		public int TotalQuantity { get; set; }
		public decimal TotalAmount { get; set; }
		public List<OrderItemViewModel> OrderItems { get; set; }
	}
}
