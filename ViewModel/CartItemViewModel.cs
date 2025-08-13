namespace MegaStor.ViewModel
{
	public class CartItemViewModel
	{
		public int CarItemId { get; set; }
		public string? NameItem { get; set; }
		public int  Quantity { get; set; }
		public decimal  TotalPricForItems { get; set; }
		public string? DescriptionProductItem { get; set; }
		public decimal PriceItem { get; set; }
		public decimal pricediscount { get; set; }
		public string? ImageItem { get; set; }
		public string? SelerName { get; set; }
 
	}
}
