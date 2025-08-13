using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
	public class SubCategory
	{
		[Key]
 		public string SubCategoryId { get; set; }

		public SubCategory()
		{
			SubCategoryId = Guid.NewGuid().ToString();
		}

		public string? CategoryName { get; set; }

		public string? ImgUrl { get; set; }

		// Relationships
		[ForeignKey("Category")]
		public int? CategoryId { get; set; }
		public Category? Category { get; set; }


 		public ICollection<Product> Products { get; set; } = new List<Product>();

	}
}
