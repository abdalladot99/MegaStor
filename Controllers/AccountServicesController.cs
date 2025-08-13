using BritishGarments.Data.Services;
using MegaStor.Models;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.Controllers
{
	[Authorize]
	public class AccountServicesController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly AccountServices _accountServices;

		public AccountServicesController(UserManager<ApplicationUser> Usermanager, SignInManager<ApplicationUser> SignInManager, AccountServices accountServices)
		{
			_userManager = Usermanager;
			_signInManager = SignInManager;
			_accountServices = accountServices;
		}
		  

		// Display user's profile details	
		[HttpGet]
		public async Task<IActionResult> MyAccount()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return View("Login", "AccountSettings"); // إعادة التوجيه إلى صفحة تسجيل الدخول إذا لم يكن المستخدم مسجلاً الدخول
			}


			AccountViewModel account = new AccountViewModel();
			account.FullName = user.UserName;
			account.Email = user.Email;
			account.PhoneNumber = user.PhoneNumber;
			account.DateCreated = user.DateCreated;
			account.LastUpdated = user.LastUpdated;
			account.ProfileImageUrl = user.ImgUser;

			var roles = await _userManager.GetRolesAsync(user);//return list<string> because user can have many roles

			account.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";//using string.Join to convert list to string

			return View(account);
		}

		  


		[HttpGet]
		public async Task<IActionResult> ChangePassword()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound();
			}

			var viewModel = new ChangePasswordViewModel
			{
				UserId = user.Id,
			};
			ViewBag.Action = "Change Password";

			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);

			if (user == null)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				var viewModel = new ChangePasswordViewModel
				{
					UserId = user.Id
				};

				return View(viewModel);
			}
			var result = await _accountServices.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);

			if (result.Succeeded)
			{
				await _signInManager.RefreshSignInAsync(user);
				TempData["SuccessMessage"] = "Password Changed Successfully";
				return RedirectToAction("ChangePassword");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			var viewmodel = new ChangePasswordViewModel
			{
				UserId = user.Id,
			};

			return View(viewmodel);
		}





		[HttpGet]
		public async Task<IActionResult> EditProfile()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return NotFound();
			}

			ViewBag.Action = "Edit Profile";
			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> EditProfile(ApplicationUser user, IFormFile? ProfilePic)
		{
			if (ModelState.IsValid)
			{
				// Update user's details
				var result = await _accountServices.EditProfileAsync(user, ProfilePic);

				if (result.Succeeded)
				{
					return RedirectToAction("MyAccount");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}

			return View(user);
		}

		 



	}
}
