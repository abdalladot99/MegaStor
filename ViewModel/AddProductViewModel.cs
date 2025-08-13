using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class AddProductViewModel
	{
		public string ProductName { get; set; }
		public string ProductDescription { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }

		public decimal? discount { get; set; } 

         public int? categoryId { get; set; }
		
		[Required]
        public String SubcategoryId { get; set; }          

        [NotMapped]
		public IFormFile ClientFile { get; set; }


		[Display(Name = "Product Images")]
		public List<IFormFile>? Images { get; set; }



	}
}
