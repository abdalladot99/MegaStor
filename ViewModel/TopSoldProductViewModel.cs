using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class TopSoldProductViewModel
	{
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }
		public List<ProductImage> ProductImage { get; set; }
		public string? Category { get; set; }
		public ApplicationUser? Vendor { get; set; }
		public int TotalSold { get; set; }
	}
}
