using System.ComponentModel.DataAnnotations;
 
namespace MegaStor.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? ImgUrl { get; set; }


        public ICollection<Product> Products { get; set; } = new List<Product>();
		public ICollection<SubCategory> SubCategories { get; set; } =new List<SubCategory>();  

	}
}
 
 

 