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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public ProductsController(ApplicationDbContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.products.Include(p => p.category);
            return View(await applicationDbContext.ToListAsync());
        }
        public ActionResult GetCategories()
        {
            return View(_context.categories.ToList());
        }
        public async Task<IActionResult> AssignProducts(int id)
        {
            return View(_context.products.Where(e => e.categoryId == id).ToList());
        }

        [HttpPost]

        public async Task<IActionResult> AssignProducts(List<Product> plist)
        {
            foreach (Product p in plist)
            {
                if(p.check)
                {
                    Cart c = new Cart();
                    int qty = int.Parse(Request.Form["qty"].ToString());
                    c.productId = p.id;
                    c.totalCartPrice = (qty * p.productPrice);
                    c.categoryId = p.categoryId;
                    c.timeStamp = DateTime.UtcNow;
                    c.userName = User.Identity.Name;
                    c.productQuantity = qty;
                    _context.carts.Add(c);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Carts");
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["categoryId"] = new SelectList(_context.categories, "id", "categoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,productName,productDesc,categoryId,productPrice,imageURL,check")] Product product, IFormFile image)
        {
            if (image != null)
            {
                var name = Path.Combine(_env.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                await image.CopyToAsync(new FileStream(name, FileMode.Create));
                product.imageURL = "Images/" + image.FileName;
            }
            _context.Add(product);
            int reOrderLevel = int.Parse(Request.Form["reOrderLevel"].ToString());
            int quantity = int.Parse(Request.Form["quantity"].ToString());
            _context.Add(product);
            await _context.SaveChangesAsync();
            Inventory inv = new Inventory()
            {
                productId = product.id,
                categoryId = product.categoryId,
                reOrderLevel = reOrderLevel,
                quantity = quantity,
            };
            _context.Add(inv);
            ////var searchProduct = _context.products.FirstOrDefault(c => c.productName == product.productName);
            ////if (searchProduct != null)
            ////{
            ////    ViewBag.message = "The product already exists!";
            ////    ViewData["categoryId"] = new SelectList(_context.categories, "id", "categoryName");
            ////    return View(product);
            ////}
            //_context.Add(product);    
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["categoryId"] = new SelectList(_context.categories, "id", "categoryName", product.categoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,productName,productDesc,categoryId,productPrice,imageURL,check")] Product product)
        {
            if (id != product.id)
            {
                return NotFound();
            }
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.id))
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

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.products'  is null.");
            }
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                _context.products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.products?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
