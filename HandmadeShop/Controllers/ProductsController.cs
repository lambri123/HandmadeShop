
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HandmadeShop.Models;
using System.Linq;
using System.Threading.Tasks;
using HandmadeShop.Data;
using Microsoft.AspNetCore.Authorization;

namespace HandmadeShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(string nameSearch, string categorySearch, string priceSearch, int? pageIndex, string sortOrder)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); 
            }

            
            if (User.IsInRole("Guest"))
            {
                return RedirectToAction("Index", "Home"); 
            }

          
            pageIndex ??= 1;
            var pageSize = 10;

            var productsQuery = _context.Products.AsQueryable();

            
            if (!string.IsNullOrEmpty(nameSearch))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(nameSearch));
            }

            
            if (!string.IsNullOrEmpty(categorySearch))
            {
                productsQuery = productsQuery.Where(p => p.Category.Contains(categorySearch));
            }

            
            if (!string.IsNullOrEmpty(priceSearch) && decimal.TryParse(priceSearch, out var price))
            {
                productsQuery = productsQuery.Where(p => p.Price <= price);  
            }

          
            switch (sortOrder)
            {
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Name);
                    break;
                case "category_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Category);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
            }

            
            var totalCount = await productsQuery.CountAsync();
            var products = await productsQuery
                .Skip((pageIndex.Value - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginatedList<Product>(products, totalCount, pageIndex.Value, pageSize);

           
            ViewData["NameSearch"] = nameSearch;
            ViewData["CategorySearch"] = categorySearch;
            ViewData["PriceSearch"] = priceSearch; 
            ViewData["SortOrder"] = sortOrder;

            ViewData["NameSortParm"] = sortOrder == "name_desc" ? "name_asc" : "name_desc";
            ViewData["CategorySortParm"] = sortOrder == "category_desc" ? "category_asc" : "category_desc";
            ViewData["PriceSortParm"] = sortOrder == "price_desc" ? "price_asc" : "price_desc";  // Добавяме и сортиране по цена

            return View(model);
        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // Ограничен достъп за роли
            if (User.IsInRole("Guest"))
            {
                return RedirectToAction("Index", "Home"); 
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

       
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,Price,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

       
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home"); 
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Price,Description")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // Формата за изтриване на продукт
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home"); // Само администратори могат да изтриват
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Изтриване на продукт
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}


