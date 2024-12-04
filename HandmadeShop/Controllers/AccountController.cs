




using HandmadeShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
    }

   
    [HttpGet]
    public IActionResult Register()
    {
        _logger.LogInformation("Потребителят отваря страницата за регистрация.");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Потребителят с потребителско име {UserName} е регистриран успешно.", model.UserName);

               
                var roleExist = await _roleManager.RoleExistsAsync("User");
                if (!roleExist)
                {
                   
                    var role = new ApplicationRole { Name = "User" };
                    await _roleManager.CreateAsync(role);
                }

                // всеки нов потребител получава User
                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogWarning("Грешка при регистрацията на потребителя {UserName}: {ErrorDescription}", model.UserName, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        _logger.LogWarning("Регистрацията на потребителя {UserName} е неуспешна.", model.UserName);
        return View(model);
    }

    
    [HttpGet]
    public IActionResult Login()
    {
        _logger.LogInformation("Потребителят отваря страницата за вход.");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Потребителят с потребителско име {UserName} влезе успешно.", model.UserName);
                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("Невалидно потребителско име или парола за потребителя {UserName}.", model.UserName);
            ModelState.AddModelError(string.Empty, "Невалидно потребителско име или парола.");
        }

        return View(model);
    }

 
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Започва процедурата по изход.");
        return await LogoutInternal();
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost()
    {
        _logger.LogInformation("Извикване на POST заявка за изход.");
        return await LogoutInternal();
    }

  
    private async Task<IActionResult> LogoutInternal()
    {
        try
        {
            var userName = User.Identity?.Name ?? "неидентифициран";
            _logger.LogInformation("Потребителят {UserName} се опитва да излезе.", userName);

            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            _logger.LogInformation("Потребителят {UserName} успешно излезе от системата.", userName);

            return RedirectToAction("LogoutConfirmation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Грешка при излизане на потребителя.");
            return RedirectToAction("Error", "Home");
        }
    }

    // потвърждаване на изход
    [HttpGet]
    public IActionResult LogoutConfirmation()
    {
        _logger.LogInformation("Потребителят достигна страницата за потвърждение на изход.");
        return View(); 
    }
}



