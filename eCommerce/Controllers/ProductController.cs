using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace eCommerce.Controllers;

public class ProductController : Controller
{
    private readonly ProductDbContext _context;
    public ProductController(ProductDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(string? title = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        // Start with the full product set and apply filters conditionally
        IQueryable<Product> query = _context.Products;

        if (!string.IsNullOrWhiteSpace(title))
        {
            // Case-insensitive search on Title
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{title}%"));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        var results = await query.OrderBy(p => p.ProductId).ToListAsync();
        return View(results);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost] 
    public async Task<IActionResult> Create(Product p)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(p);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{p.Title} was created successfully!"; 

            return RedirectToAction(nameof(Index));
        }
        return View(p);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        Product? product = _context.Products.Where(p => p.ProductId == id)
            .FirstOrDefault();

        if (product == null)
        {
            return NotFound();
        }

        return View(product); 
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Update(product); 
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{product.Title} was updated successfully"; 
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {

        if (id <= 0)
        {
            return BadRequest(); 
        }

        Product? product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [ActionName("Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        Product? product = await _context.Products.FindAsync(id);
            

        if (product == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _context.Remove(product);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"{product.Title} was successfully deleted";
        return RedirectToAction(nameof(Index));
    }
}
