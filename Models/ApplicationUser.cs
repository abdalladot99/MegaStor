using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MegaStor.Models
{
    public class ApplicationUser : IdentityUser    
    {
        public string? Address { get; set; }
        public string? ImgUser { get; set; }
	    public string? FullName { get; set; }
		public DateTime? DateCreated { get; set; }
		public DateTime? LastUpdated { get; set; }
		 
		[InverseProperty("ApplicationUser")]
  		public Cart? Cart { get; set; }      // One-to-One with Cart
		public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
 


    }

}
