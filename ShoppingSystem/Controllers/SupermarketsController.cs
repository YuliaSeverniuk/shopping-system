using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Data;
using ShoppingSystem.Models;

namespace ShoppingSystem.Controllers
{
    public class SupermarketsController : Controller
    {
        private const int PageSize = 3;
        private readonly ShoppingContext _context;

        public SupermarketsController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Supermarkets?pageNumber=2
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var supermarkets = _context.Supermarkets.AsNoTracking().OrderBy(s => s.Name);
            var page = pageNumber is > 0 ? pageNumber.Value : 1;
            return View(await PaginatedList<Supermarket>.CreateAsync(supermarkets, page, PageSize));
        }

        // GET: Supermarkets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var supermarket = await _context.Supermarkets.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (supermarket == null) return NotFound();

            return View(supermarket);
        }

        // GET: Supermarkets/Create
        public IActionResult Create() => View();

        // POST: Supermarkets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address")] Supermarket supermarket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supermarket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supermarket);
        }

        // GET: Supermarkets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var supermarket = await _context.Supermarkets.FindAsync(id);
            if (supermarket == null) return NotFound();

            return View(supermarket);
        }

        // POST: Supermarkets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Supermarket supermarket)
        {
            if (id != supermarket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supermarket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Supermarkets.Any(e => e.Id == supermarket.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supermarket);
        }

        // GET: Supermarkets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var supermarket = await _context.Supermarkets.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (supermarket == null) return NotFound();

            ViewData["HasOrders"] = await _context.Orders.AnyAsync(o => o.SuperMarketId == id);
            return View(supermarket);
        }

        // POST: Supermarkets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supermarket = await _context.Supermarkets.FindAsync(id);
            if (supermarket != null)
            {
                if (await _context.Orders.AnyAsync(o => o.SuperMarketId == id))
                {
                    ViewData["HasOrders"] = true;
                    return View(supermarket);
                }
                _context.Supermarkets.Remove(supermarket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
