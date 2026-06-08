using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechMoveGLMS.Shared.Data;


// I created this factory to run integration tests with an in‑memory database instead of the real SQL Server.
// The factory removes the original DbContext registration and replaces it with UseInMemoryDatabase.

namespace TechMoveGLMS.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Force the environment to "Testing" so that the real SQL Server is skipped
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            // Add in-memory database (the real one is skipped because environment is "Testing")
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
            
            // Ensure database is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }
}


// Microsoft, 2026. Integration tests with WebApplicationFactory.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests