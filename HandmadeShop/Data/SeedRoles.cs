using HandmadeShop.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace HandmadeShop.Data
{
    public static class SeedRoles
    {
        
        public static async Task Initialize(RoleManager<ApplicationRole> roleManager)
        {
            
            var roles = new string[] { "Admin", "User", "Guest" };

            
            foreach (var roleName in roles)
            {
                
                if (await roleManager.RoleExistsAsync(roleName) == false)
                {
                    
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper() 
                    };

                   
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}

