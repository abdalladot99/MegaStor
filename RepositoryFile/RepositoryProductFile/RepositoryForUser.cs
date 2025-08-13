using MegaStor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MegaStor.RepositoryFile.RepositoryProductFile
{
    public class RepositoryForUser
    {
         private readonly UserManager<ApplicationUser> _userManager;

        public RepositoryForUser( UserManager<ApplicationUser> userManager)
        {
             _userManager = userManager;
        }
         
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }



        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }


        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }


         






        ////////////////////////////////////////

        //public async Task<ApplecationUser> GetUserByIdAsync(string userId)
        //{
        //    return await _userManager.FindByIdAsync(userId);
        //}

        //public async Task<ApplecationUser> GetUserByEmailAsync(string email)
        //{
        //    return await _userManager.FindByEmailAsync(email);
        //}

        //public async Task<IdentityResult> CreateUserAsync(ApplecationUser user, string password)
        //{
        //    return await _userManager.CreateAsync(user, password);
        //}

        //public async Task<IdentityResult> UpdateUserAsync(ApplecationUser user)
        //{
        //    return await _userManager.UpdateAsync(user);
        //}

        //public async Task<IdentityResult> DeleteUserAsync(ApplecationUser user)
        //{
        //    return await _userManager.DeleteAsync(user);
        //}

        //public async Task<IdentityResult> ChangePasswordAsync(ApplecationUser user, string currentPassword, string newPassword)
        //{
        //    return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        //}

        //public async Task<IdentityResult> ResetPasswordAsync(ApplecationUser user, string token, string newPassword)
        //{
        //    return await _userManager.ResetPasswordAsync(user, token, newPassword);
        //}

        //public async Task<string> GeneratePasswordResetTokenAsync(ApplecationUser user)
        //{
        //    return await _userManager.GeneratePasswordResetTokenAsync(user);
        //}

        //public async Task<IList<string>> GetUserRolesAsync(ApplecationUser user)
        //{
        //    return await _userManager.GetRolesAsync(user);
        //}

        //public async Task<IdentityResult> AddUserToRoleAsync(ApplecationUser user, string role)
        //{
        //    return await _userManager.AddToRoleAsync(user, role);
        //}

        //public async Task<IdentityResult> RemoveUserFromRoleAsync(ApplecationUser user, string role)
        //{
        //    return await _userManager.RemoveFromRoleAsync(user, role);
        //}

        //public async Task<bool> IsUserInRoleAsync(ApplecationUser user, string role)
        //{
        //    return await _userManager.IsInRoleAsync(user, role);
        //}

        //public async Task<string> GenerateEmailConfirmationTokenAsync(ApplecationUser user)
        //{
        //    return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //}

        //public async Task<IdentityResult> ConfirmEmailAsync(ApplecationUser user, string token)
        //{
        //    return await _userManager.ConfirmEmailAsync(user, token);
        //}

        //public async Task<bool> CheckPasswordAsync(ApplecationUser user, string password)
        //{
        //    return await _userManager.CheckPasswordAsync(user, password);
        //}






    }
}
