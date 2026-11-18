using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Data;
using ShoppingSystem.Models;

namespace ShoppingSystem.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ShoppingContext _context;

        public CustomersController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Customers?sortOrder=lname_desc&searchString=B
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["LastNameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "lname_desc" : "";
            ViewData["AddressSortParm"] = sortOrder == "address" ? "address_desc" : "address";
            ViewData["CurrentFilter"] = searchString;

            var customers = _context.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                customers = customers.Where(c => c.LastName.Contains(searchString)
                                              || c.FirstName.Contains(searchString));
            }

            customers = sortOrder switch
            {
                "lname_desc" => customers.OrderByDescending(c => c.LastName),
                "address" => customers.OrderBy(c => c.Address),
                "address_desc" => customers.OrderByDescending(c => c.Address),
                _ => customers.OrderBy(c => c.LastName),
            };

            return View(await customers.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create() => View();

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstName,Address,Discount")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstName,Address,Discount")] Customer customer)
        {
            if (id != customer.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e => e.ID == customer.ID)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
            if (customer == null) return NotFound();

            ViewData["HasOrders"] = await _context.Orders.AnyAsync(o => o.CustomerId == id);
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                if (await _context.Orders.AnyAsync(o => o.CustomerId == id))
                {
                    ViewData["HasOrders"] = true;
                    return View(customer);
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
