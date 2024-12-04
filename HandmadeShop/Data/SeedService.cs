using HandmadeShop.Models;
using HandmadeShop.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace HandmadeShop.Data
{
    public class SeedService : ISeedService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        
        public async Task SeedRolesAsync()
        {
            var roles = new string[] { "Admin", "User", "Guest" };

            foreach (var role in roles)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

      
        public async Task SeedUsersAsync()
        {
           
            var adminUser = await _userManager.FindByEmailAsync("admin@handmadeshop.com");
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    UserName = "admin@handmadeshop.com",
                    Email = "admin@handmadeshop.com",
                    FullName = "Администратор"
                };

                var result = await _userManager.CreateAsync(newAdminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }

            
        }
    }
}

