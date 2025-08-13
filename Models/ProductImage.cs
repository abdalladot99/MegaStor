using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
	public class ProductImage
	{
		[Key]
		[MaxLength(450)]
		public string ImageId { get; set; }

		public ProductImage()
		{
			ImageId = Guid.NewGuid().ToString();
		}

		public string? ImageURL { get; set; }


		[ForeignKey("Product")]
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}
