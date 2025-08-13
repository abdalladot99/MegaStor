using MegaStor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace External_Logins.Controllers
{
	public class TestController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public TestController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		[Authorize]

		public async Task<IActionResult> Index()
		{
			var appUser = await _userManager.GetUserAsync(User);

			if (appUser == null)
				return RedirectToAction("Login");

			return View("Index", appUser);
		}
	}
}
