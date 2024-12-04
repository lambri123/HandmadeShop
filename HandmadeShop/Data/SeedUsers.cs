using Microsoft.AspNetCore.Identity;
using HandmadeShop.Models;
using System.Threading.Tasks;

namespace HandmadeShop.Data
{
    public static class SeedUsers
    {
        public static async Task Initialize(UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync("admin@example.com");
            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FullName = "Администратор" 
                };
                var result = await userManager.CreateAsync(newUser, "AdminPassword123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                }
                else
                {
                   
                    foreach (var error in result.Errors)
                    {
                        
                    }
                }
            }
        }
    }
}


