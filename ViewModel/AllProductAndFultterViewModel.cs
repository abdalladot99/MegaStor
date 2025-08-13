using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class AllProductAndFultterViewModel
	{
		public IEnumerable<Product> Products { get; set; }
		public IEnumerable<SubCategory> SubCategories { get; set; }
		public List<string> categoryIds { get; set; } = new List<string>();

	    public decimal? minPrice { get; set; }
		public decimal? maxPrice { get; set; }
		public int page { get; set; } 

		public  string? query { get; set; }
	}
}
