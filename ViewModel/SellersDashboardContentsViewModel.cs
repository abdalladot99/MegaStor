using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class SellersDashboardContentsViewModel
	{
		public IEnumerable<Product> Products { get; set; }= new List<Product>();
		public IEnumerable<OrderItem> Orders { get; set; }= new List<OrderItem>();



	}
}
