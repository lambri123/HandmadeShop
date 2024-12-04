using HandmadeShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UserRolesController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var userRolesViewModel = new List<UserRolesViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRolesViewModel.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles
            });
        }

        return View(userRolesViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Manage(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ManageUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList(),
            UserRoles = await _userManager.GetRolesAsync(user)
        };

        return View(model);
    }

    
    [HttpPost]
    public async Task<IActionResult> Manage(ManageUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = model.SelectedRoles ?? new List<string>();

        
        var rolesToAdd = selectedRoles.Except(currentRoles);
        await _userManager.AddToRolesAsync(user, rolesToAdd);

        
        var rolesToRemove = currentRoles.Except(selectedRoles);
        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        return RedirectToAction(nameof(Index));
    }
}



