using Inventory_Project.Data;
using Inventory_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalItems = await _context.Item.CountAsync();
            var totalQuantity = await _context.Item.SumAsync(i => i.Quantity);
            var totalValue = await _context.Item.SumAsync(i => i.Price * i.Quantity);

            var categoryStats = await _context.Item
                .Include(i => i.Category)
                .GroupBy(i => i.Category.Name)
                .Select(g => new CategorySummary
                {
                    CategoryName = g.Key,
                    ItemCount = g.Count(),
                    TotalQuantity = g.Sum(i => i.Quantity),
                    TotalValue = g.Sum(i => i.Price * i.Quantity)
                })
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalItems = totalItems,
                TotalQuantity = totalQuantity,
                TotalValue = totalValue,
                CategorySummary = categoryStats
            };

            return View(model);   // -> esto va a Views/Home/Index.cshtml
        }

        // deja tus otros actions (Privacy, Error, etc.) como estaban si los tienes
    }
}

