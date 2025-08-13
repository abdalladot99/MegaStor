using System.Security.Claims;
using MegaStor.AddImge;
using MegaStor.Constants.Enum;
using MegaStor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Infrastructure;

namespace MegaStor.Controllers.DifferentWaysToLogin
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly AddAndDeleteImageInServer _saveImage;
		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
			AddAndDeleteImageInServer _SaveImage)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_saveImage = _SaveImage;
		}


		#region Generic External Login 
		public IActionResult ExternalLogin(string provider)
		{
			var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		public async Task<IActionResult> ExternalLoginCallback()
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
				return RedirectToAction("Login");

			var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
			if (signInResult.Succeeded)
				return RedirectToAction("Index", "Test");

			var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? $"{info.ProviderKey}@{info.LoginProvider}.com";
			var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

			var file = new FormFile(System.IO.File.OpenRead("wwwroot/img/avatar1.jpg"),
				0, new FileInfo("wwwroot/img/avatar1.jpg").Length, "ClientFile", "avatar1.jpg")
			{ Headers = new HeaderDictionary(), ContentType = "image/jpeg" };

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				user = new ApplicationUser
				{
					UserName = email,
					Email = email,
					FullName = name,
					ImgUser = await _saveImage.SaveImageAsync(file),
					DateCreated=new DateTime(),

				};
				var createResult = await _userManager.CreateAsync(user);
				if (!createResult.Succeeded)
				{
					ModelState.AddModelError("", "Happened Error on login ");
					return RedirectToAction("Login","AccountSettings");
				}
				else  
				{
					var roleResult = await _userManager.AddToRoleAsync(user, AppRolesEnum.Customer.ToString());
				}
			}

			// ربط المستخدم بالمزود الخارجي
			var alreadyLinked = (await _userManager.GetLoginsAsync(user))
				.Any(login => login.LoginProvider == info.LoginProvider && login.ProviderKey == info.ProviderKey);

			if (!alreadyLinked)
			{
				var linkResult = await _userManager.AddLoginAsync(user, info);
				if (!linkResult.Succeeded)
				{

					ModelState.AddModelError("", "Happened Error on login ");
					return RedirectToAction("Login");
				}
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			return RedirectToAction("Index", "Test");
		}

		#endregion



	}
}
