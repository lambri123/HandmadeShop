using HandmadeShop.Data;
using HandmadeShop.Models;
using HandmadeShop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.ClearProviders();
builder.Logging.AddConsole();  
builder.Logging.AddDebug();    


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<RoleManager<ApplicationRole>>(); 
builder.Services.AddScoped<UserManager<ApplicationUser>>(); 


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";     
        options.LogoutPath = "/Account/Logout";   
        options.Events = new CookieAuthenticationEvents
        {
            OnSigningOut = async context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Потребителят се изписва успешно.");
            },
            OnSignedIn = async context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Потребителят е влязъл успешно.");
            },
            OnRedirectToLogout = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Потребителят е пренасочен към изход.");
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddControllersWithViews();


builder.Services.AddScoped<AddAdminRightsService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Започваме инициализация на ролите и потребителите...");

    try
    {
        
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await SeedRoles.Initialize(roleManager);

       
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await SeedUsers.Initialize(userManager);

        var addAdminRightsService = services.GetRequiredService<AddAdminRightsService>();
        await addAdminRightsService.AddAdminRightsToUserAsync();  

        logger.LogInformation("Ролите и потребителите са успешно инициализирани.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Грешка при инициализацията на ролите и потребителите.");
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();






