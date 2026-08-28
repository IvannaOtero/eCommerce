using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore; // <-- add this
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DBContext Configuration
builder.Services.AddDbContext<eCommerce.Data.ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
});

var app = builder.Build();

// Seed database with initial products
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var ctx = services.GetRequiredService<eCommerce.Data.ProductDbContext>();
    // Ensure database is created and migrations applied
    ctx.Database.Migrate();

    if (!ctx.Products.Any())
    {
        ctx.Products.AddRange(
            new eCommerce.Models.Product { Title = "The Great Gatsby", Price = 10.99m },
            new eCommerce.Models.Product { Title = "1984", Price = 8.99m },
            new eCommerce.Models.Product { Title = "To Kill a Mockingbird", Price = 12.50m },
            new eCommerce.Models.Product { Title = "Pride and Prejudice", Price = 9.75m },
            new eCommerce.Models.Product { Title = "The Catcher in the Rye", Price = 11.00m },
            new eCommerce.Models.Product { Title = "Moby-Dick", Price = 14.25m }
        );
        ctx.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
