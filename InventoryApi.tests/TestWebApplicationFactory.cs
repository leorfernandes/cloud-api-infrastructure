using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryApi.Data;

namespace InventoryApi.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Host=localhost;Port=5432;Database=inventorydb_test;Username=postgres;Password=postgres";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<InventoryDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Point EF Core at the test database instead
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseNpgsql(TestConnectionString));

            // Recreate the schema fresh, every time the factory starts
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            db.Database.EnsureDeleted();

            Npgsql.NpgsqlConnection.ClearAllPools();
            
            db.Database.EnsureCreated();
        });
    }
}