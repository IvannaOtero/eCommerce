using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace eCommerce.Controllers;

public class ProductController : Controller
{
    private readonly ProductDbContext _context;
    public ProductController(ProductDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        List<Product> allProducts = await _context.Products.ToListAsync();
        return View(allProducts);
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
        Product product = _context.Products
        .Where(p => p.ProductId == id)
        .FirstOrDefault();

        if (product == null)
        {
            return NotFound();
        }

        return View(product); 
    }
}
