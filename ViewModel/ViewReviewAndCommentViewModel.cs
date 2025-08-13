using MegaStor.Models;

namespace MegaStor.ViewModel
{
	public class ViewReviewAndCommentViewModel
	{
		public List<Review> ReviewAndComment { get; set; } = new List<Review>();
		public Product Product { get; set; } = new Product();
	}
}
