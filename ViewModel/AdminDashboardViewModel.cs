using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class AdminDashboardViewModel
	{
		public int TotalProducts { get; set; }
		public int TotalCustomers { get; set; }
  		public int TotalSellers { get; set; }
  		public int TotalMainAdmin { get; set; }
		public int TotalCategories { get; set; }
		public int totalSubCategories { get; set; }


		public List<int> MonthlyOrders { get; set; } // [12, 19, 8, 15,...]
		public List<decimal> MonthlySales { get; set; } // [500, 700,...]
		public List<string> MonthLabels { get; set; } // ["Jan", "Feb",...]


		public decimal TotalSales { get; set; }
		public int TotalOrders { get; set; }
		public decimal AverageOrderValue { get; set; }

		public double SalesGrowth { get; set; }
		public double OrdersGrowth { get; set; }
		public double AverageOrderGrowth { get; set; }

		public List<ProductViewModel> TopProducts { get; set; }
		 


	}


}
