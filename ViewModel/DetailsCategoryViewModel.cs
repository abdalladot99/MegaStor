using MegaStor.Models;
using System.ComponentModel.DataAnnotations;

namespace MegaStor.ViewModel
{
    public class DetailsCategoryViewModel
    {
        public string SubCategoryId { get; set; } = string.Empty;
        public int CategoryId { get; set; } = 0;

		public string Name { get; set; }

        public IFormFile? formFile { get; set; }
        public string? ImgUrl { get; set; }

 

    }
}
