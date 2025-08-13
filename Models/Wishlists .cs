using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
	public class Wishlists
	{
		[Key]
		public int Id { get; set; }

		public DateTime DateAdded { get; set; }= DateTime.Now;

		 
		[ForeignKey("User")]
		public string CustomerId { get; set; }
		public ApplicationUser? User { get; set; }


		[ForeignKey("Product")]
		public int ProductId { get; set; }
		public Product? Product { get; set; }
	}
}
