using MegaStor.Models;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
	public class AccountInformationViewComponent : ViewComponent
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public AccountInformationViewComponent(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{

			var user = await _userManager.GetUserAsync(ViewContext.HttpContext.User);
			if (user == null)
			{
				return View("Login", "AccountSettings"); // إعادة التوجيه إلى صفحة تسجيل الدخول إذا لم يكن المستخدم مسجلاً الدخول
			}


			AccountViewModel account = new AccountViewModel();
			account.FullName = user.FullName;
			account.UserName = user.UserName;
			account.Email = user.Email;
			account.PhoneNumber = user.PhoneNumber;
			account.DateCreated = user.DateCreated;
			account.LastUpdated = user.LastUpdated;
			account.ProfileImageUrl = user.ImgUser;

			var roles = await _userManager.GetRolesAsync(user);//return list<string> because user can have many roles

			account.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";//using string.Join to convert list to string

			return View(account);
 		}
		 
	}
}

