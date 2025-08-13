using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MegaStor.Constants.Enum;
 
namespace MegaStor.Models
{
    public class Order
    {
        [Key]
        public string Id { get; set; }
		public Order()
		{
			Id = Guid.NewGuid().ToString();
		}

		public DateTime OrderDate { get; set; }  

        public decimal TotalAmount { get; set; }
		public int TotalQuantity { get; set; }

 		public OrderPlacementStatusEnum OrderStatus { get; set; } = OrderPlacementStatusEnum.Pending;
		public OrderShippingStatusEnum ShippingStatus { get; set; } = OrderShippingStatusEnum.Pending;
		
		// Navigation Properties
		public ShippingAddress ShippingAddress { get; set; }
		public BillingAddress BillingAddress { get; set; }

		public Payment Payment { get; set; } // One-to-One with Payment


		[ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public ApplicationUser? Customer { get; set; } 

        public ICollection<OrderItem> OrderItems { get; set; }  

    }
}
