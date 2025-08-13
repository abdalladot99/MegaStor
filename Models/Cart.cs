using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
	 
	public class Cart
	{
		[Key]
		[MaxLength(450)]
		public string CartId { get; set; }

		public Cart()
		{
			CartId = Guid.NewGuid().ToString();
		}

		public int TotalQuantity { get; set; }



		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalAmount { get; set; }


		public string UserId { get; set; } 	
		[ForeignKey("UserId")] 
		public ApplicationUser ApplicationUser { get; set; } // One-to-One with User
		 
		public ICollection<CartItem>? CartItems { get; set; }
	}
}
