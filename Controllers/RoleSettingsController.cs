using MegaStor.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MegaStor.Controllers
{
	[Authorize(Roles = "Admin")]

	public class RoleSettingsController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
 

        public RoleSettingsController(RoleManager<IdentityRole> RoleManager )
        {
            roleManager = RoleManager;
            
        }
         

        [HttpGet]
        public IActionResult AddRole() 
        {
            return View();
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(AddRoleViewModel role) 
        {
            if (ModelState.IsValid) 
            {
                IdentityRole Role = new IdentityRole();
                Role.Name=role.RoleName;

                IdentityResult result = await roleManager.CreateAsync(Role);
                if (result.Succeeded) 
                {
                    return RedirectToAction(nameof(AddRole));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(role);
        }

          

         


    }
}
