using System.Threading.Tasks;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 
namespace MegaStor.Controllers
{
	[Authorize(Roles = "Admin,Seller")] 
	public class SellerController : Controller
	{
		private readonly IRepository<Product> _repoProduct;
		private readonly IRepository<OrderItem> _repoOrderItem;
		private readonly IRepository<Review> _repoReview;
		private readonly UserManager<ApplicationUser> _userManager;

		public SellerController(IRepository<Product> Repository,UserManager<ApplicationUser> UserManager, IRepository<OrderItem> RepoOrder, IRepository<Review> RepoReview)
		{
			_repoProduct = Repository;
			_userManager = UserManager;
			_repoOrderItem = RepoOrder;
			_repoReview = RepoReview;
		}

		public IActionResult ViewReviewAndComment(int id)
		{
			var userId = _userManager.GetUserId(User);
			var product =  _repoProduct.GetAllAsQuery().Include(r=>r.Reviews).Where(i=>i.Id==id).First();
				 
			var ReviewAndComment = _repoReview.GetAllAsQuery()
			    .Include(p=>p.Product).
				Include(p => p.Customer).
				Where(i => i.Product.VendorId == userId).
				Where(i=>i.Product.Id==id).ToList();

				if (User.IsInRole("Admin")) 
				{
					 ReviewAndComment = _repoReview.GetAllAsQuery()
					.Include(p=>p.Product).
					Include(p => p.Customer).
 					Where(i=>i.Product.Id==id).ToList(); 
				}

			var ViewReviewAndCommentViewModel = new ViewReviewAndCommentViewModel
			{
				ReviewAndComment = ReviewAndComment,
				Product = product
			};

			return View(ViewReviewAndCommentViewModel);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteReview(int Id) 
		{
			var review = _repoReview.GetByIdAsync(Id);
			
			if (review != null) 
			{ 
				bool result =await _repoReview.DeleteAsync(Id);
				if(result)
 					return Ok();
			}
			return BadRequest();

		}


		public IActionResult DashboardSeller()
		{
			var userId = _userManager.GetUserId(User);

			var products = _repoProduct.GetAllAsQuery()
				.Include(R=>R.Reviews).Where(S => S.VendorId == userId).
				ToList();
			
			var orders = _repoOrderItem.GetAllAsQuery()
				.Where(p => p.Product.VendorId == userId)
				.Include(p=>p.Product)
				.Include(p=>p.Order) 
					.ThenInclude(c=>c.Customer)
  				.ToList();
			 
			var viewModel = new SellersDashboardContentsViewModel
			{
 				Products = products,
				Orders= orders,
			};
			return View(viewModel);
		}




	}
}
