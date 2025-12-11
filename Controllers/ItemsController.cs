using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inventory_Project.Data;
using Inventory_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Inventory_Project.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Items
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["QuantitySortParm"] = sortOrder == "qty_asc" ? "qty_desc" : "qty_asc";
            ViewData["CategorySortParm"] = sortOrder == "cat_asc" ? "cat_desc" : "cat_asc";

            var items = _context.Item
                .Include(i => i.Category)
                .AsQueryable();

            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(i => i.ProductName);
                    break;
                case "price_asc":
                    items = items.OrderBy(i => i.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(i => i.Price);
                    break;
                case "qty_asc":
                    items = items.OrderBy(i => i.Quantity);
                    break;
                case "qty_desc":
                    items = items.OrderByDescending(i => i.Quantity);
                    break;
                case "cat_asc":
                    items = items.OrderBy(i => i.Category.Name);
                    break;
                case "cat_desc":
                    items = items.OrderByDescending(i => i.Category.Name);
                    break;
                default:
                    items = items.OrderBy(i => i.ProductName);
                    break;
            }

            return View(await items.ToListAsync());
        }

        // GET: Items/ShowSearchForm
        public IActionResult ShowSearchForm()
        {
            return View();
        }

        // POST: Items/ShowSearchResults
        [HttpPost]
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            var items = await _context.Item
                .Where(j => j.ProductName.Contains(SearchPhrase))
                .Include(i => i.Category)
                .ToListAsync();

            return View("Index", items);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        // Admin + Employee pueden crear
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(_context.Category, "ID", "Name");
            return View();
        }

        // helper para guardar imagen
        private async Task<string?> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "items");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/items/" + fileName;
        }

        // POST: Items/Create
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            item.CreatedAt = DateTime.Now;

            if (item.ImageFile != null)
            {
                item.ImagePath = await SaveImageAsync(item.ImageFile);
            }

            _context.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Edit/5
        // Admin + Employee pueden editar
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.CategoryID = new SelectList(_context.Category, "ID", "Name", item.CategoryID);
            return View(item);
        }

        // POST: Items/Edit/5
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item)
        {
            if (id != item.ID)
            {
                return NotFound();
            }

            var existingItem = await _context.Item.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Actualizar campos básicos
            existingItem.ProductName = item.ProductName;
            existingItem.ItemCode = item.ItemCode;
            existingItem.Description = item.Description;
            existingItem.Price = item.Price;
            existingItem.Quantity = item.Quantity;
            existingItem.CategoryID = item.CategoryID;

            // Imagen nueva opcional
            if (item.ImageFile != null)
            {
                existingItem.ImagePath = await SaveImageAsync(item.ImageFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Delete/5
        // SOLO Admin puede borrar
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Category)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.FindAsync(id);
            if (item != null)
            {
                _context.Item.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Export PDF – Admin + Employee
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> ExportToPdf()
        {
            var items = await _context.Item
                .Include(i => i.Category)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);

                    page.Header()
                        .Text("Inventory Report")
                        .FontSize(20)
                        .SemiBold()
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);   // ID
                            columns.RelativeColumn(3);    // Product
                            columns.RelativeColumn(2);    // Code
                            columns.RelativeColumn(2);    // Price
                            columns.RelativeColumn(2);    // Quantity
                            columns.RelativeColumn(3);    // Category
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).Text("ID");
                            header.Cell().Element(HeaderCell).Text("Product");
                            header.Cell().Element(HeaderCell).Text("Code");
                            header.Cell().Element(HeaderCell).Text("Price");
                            header.Cell().Element(HeaderCell).Text("Qty");
                            header.Cell().Element(HeaderCell).Text("Category");
                        });

                        foreach (var item in items)
                        {
                            table.Cell().Element(BodyCell).Text(item.ID.ToString());
                            table.Cell().Element(BodyCell).Text(item.ProductName ?? string.Empty);
                            table.Cell().Element(BodyCell).Text(item.ItemCode ?? string.Empty);
                            table.Cell().Element(BodyCell).Text(item.Price.ToString("C2"));
                            table.Cell().Element(BodyCell).Text(item.Quantity.ToString());
                            table.Cell().Element(BodyCell).Text(item.Category?.Name ?? string.Empty);
                        }

                        static IContainer HeaderCell(IContainer container)
                        {
                            return container
                                .PaddingVertical(5)
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Darken2)
                                .DefaultTextStyle(x => x.SemiBold());
                        }

                        static IContainer BodyCell(IContainer container)
                        {
                            return container
                                .PaddingVertical(3)
                                .BorderBottom(0.5f)
                                .BorderColor(Colors.Grey.Lighten3);
                        }
                    });

                    page.Footer()
                        .AlignRight()
                        .Text(t =>
                        {
                            t.Span("Generated ").FontSize(9);
                            t.Span(DateTime.Now.ToString("g")).FontSize(9).SemiBold();
                        });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            var fileName = $"Inventory_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ID == id);
        }
    }
}
