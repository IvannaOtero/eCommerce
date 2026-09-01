using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore; // <-- add this
using Microsoft.Extensions.DependencyInjection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// For development, place the SQLite DB in LocalAppData to avoid OneDrive locks/permission issues.
var devDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "eCommerce", "ecommerce-dev.db");
var devDbDir = Path.GetDirectoryName(devDbPath)!;
Directory.CreateDirectory(devDbDir);

// data base here! 
// Add services to the container.
builder.Services.AddControllersWithViews();

// DBContext Configuration
builder.Services.AddDbContext<eCommerce.Data.ProductDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use a lightweight file-based SQLite DB during development so the app can run without SQL Server auth.
        options.UseSqlite($"Data Source={devDbPath}");
    }
    else
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
        );
    }
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
});

var app = builder.Build();

// Run migrations and seeding in a background task so transient DB issues / retries don't block app startup.
// This keeps the site responsive while the background work attempts to migrate/seed the database.
if (builder.Environment.IsDevelopment())
{
    // For development, ensure the SQLite DB and schema exist before the app accepts requests.
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var ctx = services.GetRequiredService<eCommerce.Data.ProductDbContext>();
    var logger = services.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();

    try
    {
        ctx.Database.EnsureCreated();
    }
    catch (System.Exception ex)
    {
        logger?.LogWarning(ex, "EnsureCreated failed for the development SQLite database.");
    }

    try
    {
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
    catch (System.Exception ex)
    {
        logger?.LogWarning(ex, "Seeding failed for the development SQLite database.");
    }
}
else
{
    // Non-development: run migrations/seeding in background to avoid blocking startup.
    _ = Task.Run(async () =>
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var ctx = services.GetRequiredService<eCommerce.Data.ProductDbContext>();
        var logger = services.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();

        try
        {
            await ctx.Database.MigrateAsync();
        }
        catch (System.Exception ex)
        {
            logger?.LogWarning(ex, "Database migrations failed during background startup migration. If you changed models, add and apply a migration.");
        }

        try
        {
            if (await ctx.Database.CanConnectAsync())
            {
                if (!await ctx.Products.AnyAsync())
                {
                    ctx.Products.AddRange(
                        new eCommerce.Models.Product { Title = "The Great Gatsby", Price = 10.99m },
                        new eCommerce.Models.Product { Title = "1984", Price = 8.99m },
                        new eCommerce.Models.Product { Title = "To Kill a Mockingbird", Price = 12.50m },
                        new eCommerce.Models.Product { Title = "Pride and Prejudice", Price = 9.75m },
                        new eCommerce.Models.Product { Title = "The Catcher in the Rye", Price = 11.00m },
                        new eCommerce.Models.Product { Title = "Moby-Dick", Price = 14.25m }
                    );
                    await ctx.SaveChangesAsync();
                }
            }
            else
            {
                logger?.LogWarning("Database is not available during startup; skipping data seeding.");
            }
        }
        catch (System.Exception ex)
        {
            logger?.LogWarning(ex, "Database seeding failed during background startup.");
        }
    });
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
