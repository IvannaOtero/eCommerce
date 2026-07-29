using Microsoft.EntityFrameworkCore;

namespace eCommerce.Data;

public class ProductDBContext : DbContext
{
    public ProductDBContext(DbContextOptions options) : base(options)
    { 
    
    }

    //Entities to be tracked by DbContext 
    public DbSet<Models.Product> Products { get; set; }
}
