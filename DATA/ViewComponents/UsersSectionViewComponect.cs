using MegaStor.Models;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.DATA.ViewComponents
{
	[Authorize(Roles = "Admin")]
	public class UsersSectionViewComponent : ViewComponent
	{
		private readonly RepositoryForUser RepoUsers;
		private readonly UserManager<ApplicationUser> userManager;

		public UsersSectionViewComponent(RepositoryForUser users, UserManager<ApplicationUser> UserManager)
		{
			RepoUsers = users;
			userManager = UserManager;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{

			var users = await RepoUsers.GetAllUsersAsync();

			List<ShowUsersViewModel> Usersvm = new List<ShowUsersViewModel>();

			foreach (var i in users)
			{
				ShowUsersViewModel showUsersViewModel = new ShowUsersViewModel();

				showUsersViewModel.Id = i.Id;
				showUsersViewModel.ImgUrl = i.ImgUser;
				showUsersViewModel.UserName = i.UserName;
				showUsersViewModel.PhoneNumber = i.PhoneNumber;
				showUsersViewModel.Email = i.Email;
				showUsersViewModel.DateCreated = i.DateCreated.ToString();
				showUsersViewModel.Password = i.PasswordHash;

				var roles = await userManager.GetRolesAsync(i);

				showUsersViewModel.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";

				Usersvm.Add(showUsersViewModel);

			}
			return View(Usersvm);
 		}
	}
}
