using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
namespace MegaStor.Models
{
	public class ShippingAddress
	{
		[Key]
 		public string ShippingAddressId { get; set; }    
		public ShippingAddress()
		{
			ShippingAddressId = Guid.NewGuid().ToString();
		}
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? PhoneNumber { get; set; }
		public string? EmailAddress { get; set; }
		public string? Address { get; set; }
		public string? Country { get; set; }
		public string? StateOrProvince { get; set; }
		public string? City { get; set; }
		public string? PostalOrZipCode { get; set; }

		[ForeignKey("Order")]
 		public string? OrderId { get; set; }
		public Order Order { get; set; }
	}
}
