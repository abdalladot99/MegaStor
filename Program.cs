using System.Threading.Tasks;
using BritishGarments.Data.Services;
using MegaStor.AddImge;
using MegaStor.DATA;
using MegaStor.Models;
using MegaStor.RepositoryFile;
using MegaStor.RepositoryFile.RepositoryProductFile;
using MegaStor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace MegaStor
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<AppDbContext>(
			option => option.UseSqlServer(
			builder.Configuration.GetConnectionString("MyConection")));


			builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
			builder.Services.AddScoped<AddAndDeleteImageInServer>();
			builder.Services.AddScoped<RepositoryForUser>();
			builder.Services.AddScoped<CartRepository>();
			builder.Services.AddScoped<OrderServices>();
			builder.Services.AddScoped<AccountServices>();
			builder.Services.AddScoped<CategoryRepository>();
			builder.Services.AddScoped<ProductRepository>();
			builder.Services.AddScoped<OrderRepository>();
			builder.Services.AddTransient<ISenderEmail, EmailSender>();
			//////////////////////////////////////
 

			QuestPDF.Settings.License = LicenseType.Community;


			builder.Services.AddHttpContextAccessor(); //because we need to access for user id from any place in the application,  
													   //i using view component to access user id and i need to access it in the repository so i need to add this service  


			builder.Services.AddIdentity<ApplicationUser, IdentityRole>
			(
			   option => option.Password.RequireDigit = true
			)
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();



			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/AccountSettings/Login";
				options.AccessDeniedPath = "/AccountSettings/AccessDenied";  
			});

 
			builder.Services.Configure<PaymobSettings>(builder.Configuration.GetSection("PaymobSettings"));
			builder.Services.AddScoped<PaymobService>();
			builder.Services.AddHttpClient();



			#region Configure authentication with External Logins

			// Configure authentication with External Logins
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				//options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddCookie()
			.AddGoogle(options =>
			{
				options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
				options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? ""; ;
			})
			.AddGitHub(options =>
			{
				options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"] ?? "";
				options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"] ?? "";
				options.Scope.Add("user:email");
			})
			.AddFacebook(options =>
			{

				options.ClientId = builder.Configuration["Authentication:Facebook:AppId"] ?? "";
				options.ClientSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "";
				options.Fields.Add("name");
				//options.Fields.Add("email");
				//options.Scope.Add("email");
			})
			.AddLinkedIn(options =>
			{
				options.ClientId = builder.Configuration["Authentication:LinkedIn:ClientId"] ?? "";
				options.ClientSecret = builder.Configuration["Authentication:LinkedIn:ClientSecret"] ?? "";
				options.Scope.Add("openid");
				options.Scope.Add("profile");
				options.Scope.Add("email");
			})
			.AddMicrosoftAccount(options =>
			{
				options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? "";
				options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? "";
				options.SaveTokens = true;
				options.Scope.Clear();
				options.Scope.Add("openid");
				options.Scope.Add("email");
				options.Scope.Add("profile");
				options.Scope.Add("User.Read");
			})
 			;
			#endregion



			// Add services to the container.  
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.  
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.  
				app.UseHsts();
			}
			 
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");


			/////////////////////// Initialize the database with default data //////////////////////
			///
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				DbInitializer dbInitializer = new DbInitializer(services.GetRequiredService<AddAndDeleteImageInServer>());
				await dbInitializer.InitializeAsync(services);
			} 
			/////////////////////////////////////////////////////////////////////////////////////////


			app.Run();
		}
	}
}
