using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace MegaStor.ViewModel
{
	public class AddCategoryViewModelByName
	{
        [Required]
 		public string Name { get; set; }
 
        [NotMapped]
        public IFormFile clientFile { get; set; }

    }
}
