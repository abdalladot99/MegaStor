using System.ComponentModel.DataAnnotations;

namespace MegaStor.Models
{
	public class BillingAddress
	{
		[Key]
 		public string BillingAddressId { get; set; } 
		public BillingAddress()
		{
			BillingAddressId = Guid.NewGuid().ToString();
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

		// Relationship
		public string? OrderId { get; set; }
		public Order Order { get; set; }
	}
}
