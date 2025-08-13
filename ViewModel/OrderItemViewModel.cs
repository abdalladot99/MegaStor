using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.ViewModel
{
	public class OrderItemViewModel
	{
		public string ProductName { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotolPrice { get; set; }
	}
}
