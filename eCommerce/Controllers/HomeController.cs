using eCommerce.Data;
using eCommerce.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace eCommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductDbContext _context;
        private readonly Microsoft.Extensions.Logging.ILogger<HomeController> _logger;

        // Change this value to control how many products are shown per page
        private const int ProductsPerPage = 3;

        public HomeController(ProductDbContext context, Microsoft.Extensions.Logging.ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int page = 1)
        {
            try
            {
                var totalProducts = _context.Products.Count();

                var products = _context.Products
                    .OrderBy(p => p.ProductId)
                    .Skip((page - 1) * ProductsPerPage)
                    .Take(ProductsPerPage)
                    .ToList();

                var viewModel = new ProductListViewModel
                {
                    Products = products,
                    CurrentPage = page,
                    TotalPages = (int)System.Math.Ceiling(totalProducts / (double)ProductsPerPage),
                    ProductsPerPage = ProductsPerPage
                };

                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                // Log and return an empty product list so the site remains up when DB is unavailable.
                _logger?.LogWarning(ex, "Database access failed in HomeController.Index; returning empty product list.");

                var empty = new ProductListViewModel
                {
                    Products = System.Linq.Enumerable.Empty<Product>(),
                    CurrentPage = page,
                    TotalPages = 0,
                    ProductsPerPage = ProductsPerPage
                };

                return View(empty);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
