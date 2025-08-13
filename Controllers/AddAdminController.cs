using MegaStor.AddImge;
using MegaStor.Constants.Enum;
using MegaStor.Models;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.Controllers
{
	[Authorize(Roles = "MainAdmin")]

	public class AddAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> usermanager;
        private readonly SignInManager<ApplicationUser> signInManager;
		private readonly AddAndDeleteImageInServer saveImage;

		public AddAdminController(UserManager<ApplicationUser> Usermanager, SignInManager<ApplicationUser> SigninManager, AddAndDeleteImageInServer SaveImg)
        {
            usermanager = Usermanager;
			signInManager = SigninManager;
			saveImage = SaveImg;
		}



        public IActionResult Index()
        {
            return View();
        }





        [HttpGet]
        public IActionResult Newadmin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]     
		public async Task<IActionResult> Newadmin(RegisterAccountViewModel AccountUser)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					Email = AccountUser.Email,
					UserName = AccountUser.FirstName,
					FullName = AccountUser.FirstName + " " + AccountUser.LastName,
					PhoneNumber = AccountUser.PhoneNumber,
					ImgUser = AccountUser.ClientFile != null
							  ? await saveImage.SaveImageAsync(AccountUser.ClientFile)
							  : "avatar1.jpg"
				};


				var result = await usermanager.CreateAsync(user, AccountUser.Password);
				if (result.Succeeded)
				{
					var roleResult = await usermanager.AddToRoleAsync(user,AppRolesEnum.Admin.ToString());
					if (roleResult.Succeeded)
					{
						await signInManager.SignInAsync(user, AccountUser.RememberMe);
						return RedirectToAction(nameof(Index), "Home");
					}
					else
					{
						foreach (var error in roleResult.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
				}
			}

			return View(AccountUser);
		}














	}
}


