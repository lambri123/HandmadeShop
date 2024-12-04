using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HandmadeShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HandmadeShop.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

       
        public async Task<IActionResult> Index(string nameSearch, string sortOrder, int? pageIndex)
        {
            
            nameSearch = nameSearch ?? "";
            sortOrder = sortOrder ?? "Name";
            pageIndex = pageIndex ?? 1;

            
            var roles = _roleManager.Roles
                .Where(r => r.Name.Contains(nameSearch))
                .AsQueryable();

          
            roles = sortOrder == "Name" ? roles.OrderBy(r => r.Name) : roles.OrderByDescending(r => r.Name);

            
            var paginatedRoles = await PaginatedList<ApplicationRole>.CreateAsync(roles, pageIndex.Value, 10);

            ViewData["NameSortParm"] = sortOrder == "Name" ? "Name_Desc" : "Name"; 

            return View(paginatedRoles);  
        }

       
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationRole role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
            {
                ModelState.AddModelError("", "Role name cannot be empty");
                return View(role);
            }

            
            var existingRole = await _roleManager.FindByNameAsync(role.Name);
            if (existingRole != null)
            {
                ModelState.AddModelError("", "Role already exists");
                return View(role);
            }

            role.NormalizedName = role.Name.ToUpper(); 

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(role);
        }

      
        public async Task<IActionResult> Edit(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                return BadRequest("Role ID cannot be null or empty");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationRole updatedRole)
        {
            if (updatedRole == null || string.IsNullOrWhiteSpace(updatedRole.Id) || string.IsNullOrWhiteSpace(updatedRole.Name))
            {
                ModelState.AddModelError("", "Role ID and Name cannot be empty");
                return View(updatedRole);
            }

            var role = await _roleManager.FindByIdAsync(updatedRole.Id);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            role.Name = updatedRole.Name;
            role.NormalizedName = updatedRole.Name.ToUpper();

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(updatedRole);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                return BadRequest("Role ID cannot be null or empty");
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

