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

        // Change this value to control how many products are shown per page
        private const int ProductsPerPage = 3;

        public HomeController(ProductDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
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
