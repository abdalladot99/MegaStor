using System.ComponentModel.DataAnnotations;
using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class DetailsProductViewModel
	{
		public int ProductId { get; set; }
		[Required]
		public int Rating { get; set; }
		public string? Comment { get; set; } 
		public double AverageRating { get; set; }


		public Product? product { get; set; }
 		public IEnumerable<Product>? listProduct { get; set; }
 		public IEnumerable<Review>? TopReviews { get; set; }
		 
	}
}
