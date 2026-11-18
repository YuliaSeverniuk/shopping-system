using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Data;
using ShoppingSystem.Models;
using ShoppingSystem.Models.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ShoppingContext _context;

        public OrdersController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Orders  /  Orders/Index/5 (selected order)
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new OrderIndexData
            {
                Orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.SuperMarket)
                    .AsNoTracking()
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync()
            };

            if (id != null)
            {
                ViewData["OrderId"] = id.Value;
                viewModel.SelectedOrder = await LoadOrderWithDetails(id.Value);
            }

            return View(viewModel);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await LoadOrderWithDetails(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View(new Order { OrderDate = DateTime.Today });
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDate,CustomerId,SuperMarketId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                // straight to the order so the user can add products to it
                return RedirectToAction(nameof(Index), new { id = order.Id });
            }
            PopulateDropDownLists(order.CustomerId, order.SuperMarketId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            PopulateDropDownLists(order.CustomerId, order.SuperMarketId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,CustomerId,SuperMarketId")] Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Orders.Any(e => e.Id == order.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index), new { id = order.Id });
            }
            PopulateDropDownLists(order.CustomerId, order.SuperMarketId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await LoadOrderWithDetails(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order); // OrderDetails are removed by cascade
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private Task<Order> LoadOrderWithDetails(int id) =>
            _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.SuperMarket)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

        private void PopulateDropDownLists(object selectedCustomer = null, object selectedSupermarket = null)
        {
            var customers = _context.Customers.AsNoTracking()
                .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                .Select(c => new { c.ID, Name = c.FirstName + " " + c.LastName })
                .ToList();
            ViewData["CustomerId"] = new SelectList(customers, "ID", "Name", selectedCustomer);

            var supermarkets = _context.Supermarkets.AsNoTracking().OrderBy(s => s.Name).ToList();
            ViewData["SuperMarketId"] = new SelectList(supermarkets, "Id", "Name", selectedSupermarket);
        }
    }
}
