using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MegaStor.Constants.Enum;

namespace MegaStor.Models
{
	public class Payment
	{
		[Key]
 		public string PaymentId { get; set; }   

		public Payment()
		{
			PaymentId = Guid.NewGuid().ToString();
		}

		[Column(TypeName = "decimal(18,2)")]
		public decimal Amount { get; set; }  
		public DateTime PaymentDate { get; set; }
		public PaymentMethodsEnum PaymentMethod { get; set; }
		public string PaymentStatus { get; set; }



		// One-to-One: Each payment is associated with one order
		[ForeignKey("Order")]
		public string? OrderId { get; set; }
		public Order? Order { get; set; }



		// Foreign Key to ApplicationUser (the user who made the payment)
		[ForeignKey("ApplicationUser")]
		public string? UserId { get; set; }
		[ForeignKey("UserId")]
		public ApplicationUser? ApplicationUser { get; set; }
	}
}
