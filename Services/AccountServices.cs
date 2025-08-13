using MegaStor.AddImge;
using MegaStor.Models;
using Microsoft.AspNetCore.Identity;
using QuestPDF.Infrastructure;

namespace BritishGarments.Data.Services
{
	public class AccountServices
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _environment;
		private readonly AddAndDeleteImageInServer _saveImage;

		public AccountServices(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, AddAndDeleteImageInServer SaveImage)
		{
			_userManager = userManager;
			_environment = environment;
			_saveImage = SaveImage;
		}

		// Change Password Method
		public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found" });
			}

			var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

			return result;
		
		}

		// Edit Profile Method
		public async Task<IdentityResult> EditProfileAsync(ApplicationUser user, IFormFile ProfilePic)
		{
			var existingUser = await _userManager.FindByIdAsync(user.Id);

			if (existingUser == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			existingUser.UserName = user.UserName;
			existingUser.FullName = user.FullName;
			existingUser.Email = user.Email;
 			existingUser.PhoneNumber = user.PhoneNumber;
			existingUser.Address = user.Address;
			existingUser.LastUpdated = DateTime.Now;
 
			if (ProfilePic != null)
			{
				bool ResultDelete = _saveImage.DeleteImage(existingUser.ImgUser);
				if (ResultDelete)
					existingUser.ImgUser = await _saveImage.SaveImageAsync(ProfilePic);
			}
			else
			{
				existingUser.ImgUser = user.ImgUser;
			}

			var result = await _userManager.UpdateAsync(existingUser);

			return result;
		}
	}
}
