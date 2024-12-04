using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HandmadeShop.Models;

public static class ApplicationSeeder
{
    public static async Task SeedGuestUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      
        var roleExist = await roleManager.RoleExistsAsync("Guest");
        if (!roleExist)
        {
            var role = new IdentityRole("Guest");
            await roleManager.CreateAsync(role);
        }

       
        var user = await userManager.FindByEmailAsync("guest@example.com");

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = "guest@example.com",
                Email = "guest@example.com",
            };

            
            await userManager.CreateAsync(user, "Password123!");

            
            await userManager.AddToRoleAsync(user, "Guest");
        }
    }
}
