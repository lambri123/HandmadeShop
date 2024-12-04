using HandmadeShop.Models;
using Microsoft.AspNetCore.Identity;

namespace HandmadeShop.Services
{
    public class AddAdminRightsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AddAdminRightsService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddAdminRightsToUserAsync()
        {
            // Търси потребител с потребителско име 'nasko'
            var user = await _userManager.FindByNameAsync("nasko");

            if (user != null)
            {
                // Провери дали потребителят вече има роля "Admin"
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    // Ако не, добави ролята "Admin" към потребителя
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}


