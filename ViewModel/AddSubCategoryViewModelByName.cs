using System.ComponentModel.DataAnnotations.Schema;

namespace MegaStor.ViewModel
{
	public class AddSubCategoryViewModelByName 
	{
		public int CategoryId { get; set; }

		[NotMapped]
		public IFormFile clientFile { get; set; }

		public string? Name { get; set; }
 
	}

}
