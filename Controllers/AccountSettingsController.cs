using System.Text.Encodings.Web;
using BritishGarments.Data.Services;
using MegaStor.AddImge;
using MegaStor.Constants.Enum;
using MegaStor.Models;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.Services;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
 
namespace MegaStor.Controllers
{
     public class AccountSettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AddAndDeleteImageInServer saveImage;
		private readonly ISenderEmail _senderEmail;
		private readonly OrderRepository _orderrepo;
		private readonly AccountServices _accountServices;

		public AccountSettingsController(UserManager<ApplicationUser> Usermanager, SignInManager<ApplicationUser> SignInManager
            ,AddAndDeleteImageInServer SaveImg, ISenderEmail senderEmail, OrderRepository Orderrepo,AccountServices accountServices)
        {
			_userManager = Usermanager;
			_signInManager = SignInManager;
            saveImage = SaveImg;
			_senderEmail = senderEmail;
			_orderrepo = Orderrepo;
			_accountServices = accountServices;
		}

	 
		  
		public IActionResult AccessDenied()
		{
			return View();
		}

         [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAccountViewModel account) 
        {
            if (ModelState.IsValid)
            {
                ApplicationUser result = await _userManager.FindByEmailAsync(account.Email);
                if (result!=null)
                {
                    bool found = await _userManager.CheckPasswordAsync(result,account.Password);
                    if (found)
                    {
                        await _signInManager.SignInAsync(result,account.RememberMe);
						#region
						////  await signInManager.SignInAsync(userdb,login.RememberMe);
						////كده ضفنا الالعنوان في الكوكي 
						//List<Claim> claims = new List<Claim>();
						//claims.Add(new Claim("Address", userdb.Address));
						//await signInManager.SignInWithClaimsAsync(userdb, login.RememberMe, claims);

						//return RedirectToAction("Index", "Product");
						#endregion

						if (User.IsInRole("Admin")) 
						{
							return RedirectToAction("Dashboard", "Admin");
						}
						return RedirectToAction(nameof(Index),"Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "User and password invaled");
            }
            return View(account);
        }


		[Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index),"Home");
        }



 		[HttpGet]
        public IActionResult Register() 
        {
            return View();
        }
		
		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterAccountViewModel AccountUser)
		{
			if (ModelState.IsValid)
			{
				var file = new FormFile(System.IO.File.OpenRead("wwwroot/img/avatar1.jpg"), 
					0, new FileInfo("wwwroot/img/avatar1.jpg").Length, "ClientFile", "avatar1.jpg")
				{ Headers = new HeaderDictionary(), ContentType = "image/jpeg" };
				 

				var user = new ApplicationUser
				{
					Email = AccountUser.Email,
					UserName = AccountUser.FirstName,
					Address = AccountUser.Address,
					DateCreated = DateTime.Now,
					FullName = AccountUser.FirstName +" "+AccountUser.LastName,
					PhoneNumber = AccountUser.PhoneNumber,
					ImgUser = AccountUser.ClientFile != null
							  ? await saveImage.SaveImageAsync(AccountUser.ClientFile)
							  : await saveImage.SaveImageAsync(file)
				};
			 

				var result = await _userManager.CreateAsync(user, AccountUser.Password);
				if (result.Succeeded)
				{
 					var roleResult = await _userManager.AddToRoleAsync(user,AppRolesEnum.Customer.ToString());
					if (roleResult.Succeeded)
					{
						await _signInManager.SignInAsync(user, AccountUser.RememberMe);
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
		 






		[AllowAnonymous]
		// Helper method to send Password Reset Email
		private async Task SendForgotPasswordEmail(string? email, ApplicationUser? user)
		{
			// Generate the reset password token
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			// Build the password reset link which must include the Callback URL
			// Build the password reset link
			var passwordResetLink = Url.Action("ResetPassword", "AccountSettings",
					new { Email = email, Token = token }, protocol: HttpContext.Request.Scheme);
			//Send the Confirmation Email to the User Email Id
			await _senderEmail.SendEmailAsync(email, "Reset Your Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(passwordResetLink)}'>clicking here</a>.", true);
		}
 

		public IActionResult ForgotPassword()
		{
			return View();
		}

		// Handles HttpPost ForgotPassword
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					await SendForgotPasswordEmail(model.Email, user);

					return RedirectToAction("ForgotPasswordConfirmation", "AccountSettings");
				}

				return RedirectToAction("ForgotPasswordConfirmation", "AccountSettings");
			}

			return View(model);
		}

		// ForgotPasswordConfirmation
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		public IActionResult ResetPassword(string Token, string Email)
		{
			// If password reset token or email is null, most likely the
			// user tried to tamper the password reset link
			if (Token == null || Email == null)
			{
				ViewBag.ErrorTitle = "Invalid Password Reset Token";
				ViewBag.ErrorMessage = "The Link is Expired or Invalid";
				return View("Error");
			}
			else
			{
				ResetPasswordViewModel model = new ResetPasswordViewModel();
				model.Token = Token;
				model.Email = Email;
				return View(model);
			}
		}

		// Handles Reset Password 
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);

				if (user != null)
				{
					var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

					if (result.Succeeded)
					{
						return RedirectToAction("ResetPasswordConfirmation", "AccountSettings");
					}


					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}

					return View(model);
				}

				return RedirectToAction("ResetPasswordConfirmation", "AccountSettings");
			}

			return View(model);
		}

		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}
		 










	 }
 
}

