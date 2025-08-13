 using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class HomeIndexAllEverythingViewModel
	{ 
		public IEnumerable<Category>? categories { get; set; }
		public IEnumerable<SubCategory>? subCategories { get; set; }
		public IEnumerable<Product>? products { get; set; }
		public IEnumerable<Product>? Newproducts { get; set; } 
		public IEnumerable<Product>? TopSellingProducts { get; set; }
		public IEnumerable<Product>? TopDiscountProducts { get; set; }
		public IEnumerable<Product>? AllproductsBySkip { get; set; }
		 
    }
}
