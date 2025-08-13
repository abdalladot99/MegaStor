using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; } 

		  
		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; }
		 

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalPrice { get; set; }



		[ForeignKey("Cart")]
		public string CartId { get; set; }
		public Cart? Cart { get; set; }


		[ForeignKey("Product")]
		public int ProductId { get; set; }
		public Product? Product { get; set; }

		 


    }
}
