namespace MegaStor.ViewModel
{
	public class InvoicePdfModel
	{
		public string CustomerName { get; set; } = "";
		public string Address { get; set; } = "";
		public string Email { get; set; } = "";
		public string PhoneNumber { get; set; } = "";
		public string OrderDate { get; set; } = "";
		public string OrderStatus { get; set; } = "";
		public string ShippingStatus { get; set; } = "";
		public string TotalAmount { get; set; } = "";

		public List<InvoiceItemModel> Items { get; set; } = new();
	}

	public class InvoiceItemModel
	{
		public string ItemName { get; set; } = "";
		public string Quantity { get; set; } = "";
		public string UnitPrice { get; set; } = "";
		public string TotalPrice { get; set; } = "";
	}

}
