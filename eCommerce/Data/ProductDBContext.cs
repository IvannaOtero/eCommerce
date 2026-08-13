using Microsoft.EntityFrameworkCore;

namespace eCommerce.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {

    }

    //Entities to be tracked by DbContext 
    public DbSet<Models.Product> Products { get; set; } = null!;
}
