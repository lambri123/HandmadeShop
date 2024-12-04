using Microsoft.AspNetCore.Identity;

namespace HandmadeShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string FullName { get; set; }
    }
}
