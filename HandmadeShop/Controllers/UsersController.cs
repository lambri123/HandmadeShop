using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HandmadeShop.Models;
using HandmadeShop.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]  
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;  

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)  
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    
    public async Task<IActionResult> Index(string searchString)
    {
        var users = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            users = users.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString));
            ViewData["searchString"] = searchString;
        }

        return View(await users.ToListAsync());
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserName,Email,FullName")] ApplicationUser user, string password)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Паролата е задължителна.");
                return View(user);
            }

            if (string.IsNullOrEmpty(user.FullName))
            {
                user.FullName = "Няма име";
            }

            try
            {
                var newUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName
                };

                var result = await _userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Потребителят '{newUser.UserName}' беше създаден успешно.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Грешка при създаването на потребител: {ex.Message}");
            }
        }
        return View(user);
    }

   
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        ViewData["Roles"] = roles;
        ViewData["AvailableRoles"] = await _roleManager.Roles.ToListAsync();

        return View(user);
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,FullName")] ApplicationUser user, string newPassword, string[] selectedRoles)
    {
        if (string.IsNullOrEmpty(id) || id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;

                
                if (!string.IsNullOrEmpty(newPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                    var result = await _userManager.ResetPasswordAsync(existingUser, token, newPassword);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (updateResult.Succeeded)
                {
                    
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);
                    var rolesToRemove = currentRoles.Except(selectedRoles).ToList();
                    var rolesToAdd = selectedRoles.Except(currentRoles).ToList();

                    await _userManager.RemoveFromRolesAsync(existingUser, rolesToRemove);
                    await _userManager.AddToRolesAsync(existingUser, rolesToAdd);

                    TempData["SuccessMessage"] = "Потребителят беше актуализиран успешно.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Грешка при редактирането на потребител: {ex.Message}");
            }
        }
        return View(user);
    }


    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            ModelState.AddModelError(string.Empty, "Не е намерен потребител с този идентификатор.");
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = $"Потребителят '{user.UserName}' беше изтрит успешно.";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Потребителят не е намерен.");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Грешка при изтриването на потребител: {ex.Message}");
        }

        return RedirectToAction(nameof(Index));
    }
}
























