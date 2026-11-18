using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingSystem.Data;
using ShoppingSystem.Models;

namespace ShoppingSystem.Controllers
{
    /// <summary>
    /// Items (product + quantity) of an order. Always reached from the Orders page,
    /// so every action returns the user back to the selected order.
    /// </summary>
    public class OrderDetailsController : Controller
    {
        private readonly ShoppingContext _context;

        public OrderDetailsController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: OrderDetails/Create?orderId=5
        public async Task<IActionResult> Create(int? orderId)
        {
            if (orderId == null || !await _context.Orders.AnyAsync(o => o.Id == orderId)) return NotFound();

            await PopulateViewData(orderId.Value);
            return View(new OrderDetail { OrderId = orderId, Quantity = 1 });
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,Quantity")] OrderDetail orderDetail)
        {
            if (orderDetail.ProductId == null)
                ModelState.AddModelError(nameof(OrderDetail.ProductId), "Select a product.");

            if (ModelState.IsValid)
            {
                // If the product is already in the order, just increase its quantity.
                var existing = await _context.OrderDetails.FirstOrDefaultAsync(d =>
                    d.OrderId == orderDetail.OrderId && d.ProductId == orderDetail.ProductId);
                if (existing != null)
                    existing.Quantity = (existing.Quantity ?? 0) + (orderDetail.Quantity ?? 0);
                else
                    _context.Add(orderDetail);

                await _context.SaveChangesAsync();
                return BackToOrder(orderDetail.OrderId);
            }
            await PopulateViewData(orderDetail.OrderId ?? 0, orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return NotFound();

            await PopulateViewData(orderDetail.OrderId ?? 0, orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrderId,ProductId,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.ID) return NotFound();
            if (orderDetail.ProductId == null)
                ModelState.AddModelError(nameof(OrderDetail.ProductId), "Select a product.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.OrderDetails.Any(e => e.ID == orderDetail.ID)) return NotFound();
                    throw;
                }
                return BackToOrder(orderDetail.OrderId);
            }
            await PopulateViewData(orderDetail.OrderId ?? 0, orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var orderDetail = await _context.OrderDetails
                .Include(d => d.Product)
                .Include(d => d.Order).ThenInclude(o => o.Customer)
                .Include(d => d.Order).ThenInclude(o => o.SuperMarket)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (orderDetail == null) return NotFound();

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            int? orderId = orderDetail?.OrderId;
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
            return BackToOrder(orderId);
        }

        private IActionResult BackToOrder(int? orderId) =>
            RedirectToAction("Index", "Orders", new { id = orderId });

        private async Task PopulateViewData(int orderId, object selectedProduct = null)
        {
            var products = await _context.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
            // Show the price right in the drop-down: "Banana (45.50)"
            ViewData["ProductId"] = new SelectList(products, "Id", "NameWithPrice", selectedProduct);

            ViewData["Order"] = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.SuperMarket)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
