using System.Collections.Generic;

namespace eCommerce.Models;

public class ProductListViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int ProductsPerPage { get; set; }
}
