using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace MegaStor.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }
         
		[Required]
        public int Quantity { get; set; }


		public DateTime CreatedAt { get; set; }
		public bool IsAvailable { get; set; }
		public decimal discount { get; set; }
        public decimal DiscountPrice { get; set; }

		public string ImageUrl { get; set; }

		public ICollection<ProductImage>? ProductImages { get; set; }     // Navigation property to have multiple Images


		[NotMapped]
        public IFormFile? ClientFile { get; set; }


        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
		

        [ForeignKey("SubCategory")]
		public string? SubCategoryId { get; set; }
		public SubCategory? SubCategory { get; set; }


		[ForeignKey("Vendor")]
        public string?  VendorId { get; set; } 
        public ApplicationUser? Vendor { get; set; }


		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
 		public ICollection<Review> Reviews { get; set; }=new List<Review>();
		public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();


		[NotMapped]
		public double AverageRating => Reviews?.Any() == true
		? Math.Round(Reviews.Average(r => r.Rating), 1)
		: 0;


	}
}
