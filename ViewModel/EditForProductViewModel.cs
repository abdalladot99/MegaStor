 
namespace MegaStor.ViewModel
{
    public class EditForProductViewModel
    {
 
        public int Id { get; set; }
         public string Name { get; set; }

         public string Description { get; set; }

         public decimal Price { get; set; }

		 public decimal? discount { get; set; }
		 public decimal DiscountPrice { get; set; }  

  
         public int Quantity { get; set; }

         public string? ImageUrl { get; set; }

         public IFormFile? ClientFile { get; set; }

         public int CategoryId { get; set; }
         public String SubCategoryId { get; set; }
  
 
     }
}
