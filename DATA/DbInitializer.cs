using System.Data;
using MegaStor.AddImge;
using MegaStor.Constants.Enum;
using MegaStor.Models;
using Microsoft.AspNetCore.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace MegaStor.DATA
{

	public class DbInitializer
	{
		private readonly AddAndDeleteImageInServer _saveImage;
	 
		public DbInitializer(AddAndDeleteImageInServer SaveImage)
		{
			_saveImage = SaveImage;
		}
		public  async Task InitializeAsync(IServiceProvider serviceProvider)
		{
			// جلب الخدمات المطلوبة
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			
			 
			using var scope = serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
			// إضافة Categories و SubCategories لو مش موجودة
			if (!context.Categories.Any())
			{
				var categories = new List<Category>
				{
					new Category
					{
						Name = "Electronics",
						ImgUrl="Screenshot 2025-08-12 181637.png",
 						SubCategories = new List<SubCategory>
						{
							new SubCategory { CategoryName = "Mobile Phones" ,ImgUrl="4b9cfae0-9e69-490d-b432-822df1fce147.jpeg" },
							new SubCategory { CategoryName = "Laptops",ImgUrl="cat4.jpg" }
						}

					},
					new Category
					{
						Name = "Clothing",
						ImgUrl="istockphoto-1293366109-612x612.jpg", 
						SubCategories = new List<SubCategory>
						{
							new SubCategory { CategoryName = "Men",ImgUrl="suit-2737910_1280.jpg" },
							new SubCategory { CategoryName = "Women",ImgUrl="0000554_cotton-long-sleeve-casual-shirt_360.jpeg" }
						}
					}
				};

				context.Categories.AddRange(categories);
				await context.SaveChangesAsync();
			}

			  

			// 1. أضف الرولز بناءً على enum UserRole
			foreach (AppRolesEnum role in Enum.GetValues(typeof(AppRolesEnum)))
			{
				string roleName = role.ToString();

				if (!await roleManager.RoleExistsAsync(roleName))
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			var file = new FormFile(System.IO.File.OpenRead("wwwroot/img/avatar1.jpg"),
				0, new FileInfo("wwwroot/img/avatar1.jpg").Length, "ClientFile", "avatar1.jpg")
			{ Headers = new HeaderDictionary(), ContentType = "image/jpeg" };

			// 2. أضف مستخدم لكل رول لو مش موجود
			foreach (AppRolesEnum role in Enum.GetValues(typeof(AppRolesEnum)))
			{
				string userName = role.ToString().ToLower() + "_user";
				string userEmail = $"{userName}@example.com";

				var user = await userManager.FindByNameAsync(userName);
				if (user == null)
				{
					user = new ApplicationUser
					{
						UserName = userName,
						Email = userEmail,
						EmailConfirmed = true,
						ImgUser = await _saveImage.SaveImageAsync(file),
						DateCreated = new DateTime(),

					};

 					var result = await userManager.CreateAsync(user, "Password@123");

					if (result.Succeeded)
					{
						await userManager.AddToRoleAsync(user, role.ToString());
						if (role.ToString().Equals(AppRolesEnum.MainAdmin.ToString()))
						{
							await userManager.AddToRoleAsync(user,AppRolesEnum.Admin.ToString()); 
						}
					}
					else
					{
 						throw new Exception($"Failed to create user {userName}");
					}
				}
			}
		}

	}
}
