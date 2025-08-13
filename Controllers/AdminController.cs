using MegaStor.AddImge;
using MegaStor.Constants.Enum;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.Services;
using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace MegaStor.Controllers
{
	[Authorize(Roles = "Admin,MainAdmin")]
	public class AdminController : Controller
	{
		private readonly IRepository<Product> _contextProduct;
		private readonly IRepository<Category> _contextCategory;
		private readonly IRepository<SubCategory> _contextSubCategory;
		private readonly IRepository<Order> _contextOrder;
		private readonly IRepository<ApplicationUser> _contextApplecationUser;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RepositoryForUser _repoUser;
 		private readonly AddAndDeleteImageInServer _saveImage;
		private readonly OrderServices _servicesOfOrder;

		public AdminController(IRepository<Product> context, IRepository<Category> contextCategory, IRepository<Order> contextOrder,
			IRepository<ApplicationUser> contextApplecationUser, RepositoryForUser RepoUser, UserManager<ApplicationUser> userManager,
			IRepository<SubCategory> ContextSubCategory, AddAndDeleteImageInServer SaveImage, OrderServices ServicesOfOrder)
		{
			_contextProduct = context;
			_contextCategory = contextCategory;
		    _contextOrder = contextOrder;
			_contextApplecationUser = contextApplecationUser;
			_repoUser = RepoUser;
			_userManager = userManager;
			_contextSubCategory = ContextSubCategory;
			_saveImage = SaveImage;
			_servicesOfOrder = ServicesOfOrder;
		}

		public async Task<IActionResult> Dashboard()
		{
			var Products = await _contextProduct.GetAllAsync();
			var totalProducts = Products.Count();

			var Categories = await _contextCategory.GetAllAsync();
			var totalCategories = Categories.Count();

 			var SubCategories = await _contextSubCategory.GetAllAsync();
			var totalSubCategories = SubCategories.Count();

			var Users = await _contextApplecationUser.GetAllAsync();
			List<ApplicationUser> SellerList = new List<ApplicationUser>();
			List<ApplicationUser> CustomerList = new List<ApplicationUser>();
			List<ApplicationUser> MainAdmin = new List<ApplicationUser>();
 	 
			foreach (var user in Users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				if (roles.Contains(AppRolesEnum.Seller.ToString()))
					SellerList.Add(user);

				if (roles.Contains(AppRolesEnum.Customer.ToString()))
					CustomerList.Add(user);

				if (roles.Contains(AppRolesEnum.MainAdmin.ToString()))
					MainAdmin.Add(user); 
			}

			var totalSeller = SellerList.Count();
			var totalCustomer = CustomerList.Count();
			var TotalMainAdmin = MainAdmin.Count();
 			 

			var currentMonthOrders = await _contextOrder.GetAllAsync();
			decimal totalSales = currentMonthOrders.Sum(o => o.TotalAmount);


			var lastMonthOrders = await _contextOrder.GetAllAsync();


			decimal averageOrder = currentMonthOrders.Any() ? currentMonthOrders.Average(o => o.TotalAmount) : 0;

			 
			var currentMonth = DateTime.Now.Month;
			var lastMonth = DateTime.Now.AddMonths(-1).Month;
			 

			// نسب النمو
			decimal lastMonthSales = lastMonthOrders.Sum(o => o.TotalAmount);
			int lastMonthCount = lastMonthOrders.Count();


			double salesGrowth = lastMonthSales > 0 ? ((double)(totalSales - lastMonthSales) / (double)lastMonthSales) * 100 : 0;
			double orderGrowth = lastMonthCount > 0 ? ((double)(currentMonthOrders.Count() - lastMonthCount) / lastMonthCount) * 100 : 0;

			double avgOrderGrowth = (lastMonthOrders.Any() && lastMonthOrders.Average(o => o.TotalAmount) > 0)
				? ((double)(averageOrder - lastMonthOrders.Average(o => o.TotalAmount)) / (double)lastMonthOrders.Average(o => o.TotalAmount)) * 100
				: 0; 

			var model = new AdminDashboardViewModel
			{
				TotalProducts = totalProducts,
				TotalCategories= totalCategories,
				TotalSellers=totalSeller,
				TotalCustomers=totalCustomer,
				TotalMainAdmin = TotalMainAdmin,
 				totalSubCategories = totalSubCategories,
				TotalOrders = currentMonthOrders.Count(),
				TotalSales = totalSales,

				MonthLabels = _servicesOfOrder.MonthLabels(), 
				MonthlySales = _servicesOfOrder.MonthlySales(), 
				MonthlyOrders = _servicesOfOrder.MonthlyOrders(),
				 
				AverageOrderValue = averageOrder,
				SalesGrowth = Math.Round(salesGrowth, 1),
				OrdersGrowth = Math.Round(orderGrowth, 1),
				AverageOrderGrowth = Math.Round(avgOrderGrowth, 1),
 			};

			 
			return View(model);
		}
		 

		[HttpGet]
		public async Task<IActionResult> ChangeUserRole(string Id)
		{
			var users = await _repoUser.GetUserByIdAsync(Id);
			 
				ShowUsersViewModel showUsersViewModel = new ShowUsersViewModel();

				showUsersViewModel.Id = users.Id;
				showUsersViewModel.ImgUrl = users.ImgUser;
				showUsersViewModel.UserName = users.FullName;
				showUsersViewModel.PhoneNumber = users.PhoneNumber;
				showUsersViewModel.Email = users.Email;
				showUsersViewModel.DateCreated = users.DateCreated.ToString();
				var roles = await _userManager.GetRolesAsync(users);
				showUsersViewModel.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";
				var rolesList = Enum.GetValues(typeof(AppRolesEnum))
						.Cast<AppRolesEnum>()
						.Select(r => new SelectListItem
						{
							Text = r.ToString(),
							Value = r.ToString()
						}).ToList();

				ViewBag.Roles = rolesList;


			return View(showUsersViewModel);
		}
		[HttpPost]
		public async Task<IActionResult> ChangeUserRole(ShowUsersViewModel NewRole)
		{
			if (string.IsNullOrEmpty(NewRole.Id) || string.IsNullOrEmpty(NewRole.Role))
				return BadRequest("User ID or Role is missing");

			var user = await _userManager.FindByIdAsync(NewRole.Id);

			if (user == null)
				return NotFound("User not found");

 			var currentRoles = await _userManager.GetRolesAsync(user);

 			var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
			if (!removeResult.Succeeded)
				return BadRequest("Failed to remove old roles");

 			var addResult = await _userManager.AddToRoleAsync(user, NewRole.Role);
			if (!addResult.Succeeded)
				return BadRequest("Failed to add new role");
 			return Ok("Role updated successfully");
		}




		[HttpGet]
		public async Task<IActionResult> AddRoleForUser(string Id)
		{
			var users = await _repoUser.GetUserByIdAsync(Id);
			 
				ShowUsersViewModel showUsersViewModel = new ShowUsersViewModel();

				showUsersViewModel.Id = users.Id;
				showUsersViewModel.ImgUrl = users.ImgUser;
				showUsersViewModel.UserName = users.FullName;
				showUsersViewModel.PhoneNumber = users.PhoneNumber;
				showUsersViewModel.Email = users.Email;
				showUsersViewModel.DateCreated = users.DateCreated.ToString();
				var roles = await _userManager.GetRolesAsync(users);
				showUsersViewModel.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";
				var rolesList = Enum.GetValues(typeof(AppRolesEnum))
						.Cast<AppRolesEnum>()
						.Select(r => new SelectListItem
						{
							Text = r.ToString(),
							Value = r.ToString()
						}).ToList();

				ViewBag.Roles = rolesList;


			return View(showUsersViewModel);
		} 

		[HttpPost]
		public async Task<IActionResult> AddRoleForUser(ShowUsersViewModel NewRole)
		{
			if (string.IsNullOrEmpty(NewRole.Id) || string.IsNullOrEmpty(NewRole.Role))
				return BadRequest("User ID or Role is missing");

			var user = await _userManager.FindByIdAsync(NewRole.Id);
			if (user == null)
				return NotFound("User not found");

			var currentRoles = await _userManager.GetRolesAsync(user);

			// لو عنده الرول الجديد خلاص ما نضيفوش
			if (currentRoles.Contains(NewRole.Role))
				return BadRequest("User already has this role");

			var addResult = await _userManager.AddToRoleAsync(user, NewRole.Role);
			if (!addResult.Succeeded)
				return BadRequest("Failed to add new role");

			return Ok("Role added successfully");

		}






		public async Task<IActionResult> GetAllUsers()
		{

			var users = await _repoUser.GetAllUsersAsync();

			List<ShowUsersViewModel> Usersvm = new List<ShowUsersViewModel>();

			foreach (var i in users)
			{
				ShowUsersViewModel showUsersViewModel = new ShowUsersViewModel();

				showUsersViewModel.Id = i.Id;
				showUsersViewModel.ImgUrl = i.ImgUser;
				showUsersViewModel.UserName = i.FullName;
				showUsersViewModel.PhoneNumber = i.PhoneNumber;
				showUsersViewModel.Email = i.Email;
				showUsersViewModel.DateCreated = i.DateCreated.ToString();
				showUsersViewModel.Password = i.PasswordHash;

				var roles = await _userManager.GetRolesAsync(i);

				showUsersViewModel.Role = roles.Any() ? string.Join(", ", roles) : "No Role Assigned";

				Usersvm.Add(showUsersViewModel);

			}
			return View(Usersvm);
		}

		 

		[HttpPost]
		public async Task<IActionResult> DeleteUser(string Id)
		{
			var found = await _repoUser.GetUserByIdAsync(Id);
			if (found == null)
				return NotFound();

			IdentityResult result = await _repoUser.DeleteUserAsync(found);
			if (result.Succeeded)
			{
				var boole = _saveImage.DeleteImage(found.ImgUser);

				return Ok();
			}

			return BadRequest();

		}






	}
}
