using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.Models
{
    public class OrderItem
    {
        [Key]
        public string Id { get; set; }
		public OrderItem()
		{
			Id= Guid.NewGuid().ToString();
		}
		public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
 		public decimal TotalPrice { get; set; } 



		[ForeignKey("Order")]
        public string OrderId { get; set; }
        public Order? Order { get; set; }



        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }



    }
}
