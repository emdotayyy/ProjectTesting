using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTesting.Data;
using ProjectTesting.Models;

namespace ProjectTesting.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.inventories.Include(i => i.category).Include(i => i.product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventories
                .Include(i => i.category)
                .Include(i => i.product)
                .FirstOrDefaultAsync(m => m.id == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            ViewData["categoryId"] = new SelectList(_context.categories, "id", "categoryName");
            ViewData["productId"] = new SelectList(_context.products, "id", "productName");
            return View();
        }
        public JsonResult GetProducts(int sid)//categoryId as input
        {
            var products = _context.products.Where(e => e.categoryId == sid).Select(e => new //will show the products related to the particular categoryId
            {
                id = e.id,
                text = e.productName
            });
            return Json(products);
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,quantity,reOrderLevel,categoryId,productId")] Inventory inventory)
        {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            ViewData["categoryId"] = new SelectList(_context.categories, "id", "categoryName", inventory.categoryId);
            ViewData["productId"] = new SelectList(_context.products, "id", "productName", inventory.productId);
            return View(inventory);
        }
        public JsonResult EditProducts(int sid)//categoryId as input
        {
            var products = _context.products.Where(e => e.categoryId == sid).Select(e => new //will show the products related to the particular categoryId
            {
                id = e.id,
                text = e.productName
            });
            return Json(products);
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,quantity,reOrderLevel,categoryId,productId")] Inventory inventory)
        {
            if (id != inventory.id)
            {
                return NotFound();
            }
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.id))
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

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.inventories == null)
            {
                return NotFound();
            }

            var inventory = await _context.inventories
                .Include(i => i.category)
                .Include(i => i.product)
                .FirstOrDefaultAsync(m => m.id == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.inventories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.inventories'  is null.");
            }
            var inventory = await _context.inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.inventories.Remove(inventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
          return (_context.inventories?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
