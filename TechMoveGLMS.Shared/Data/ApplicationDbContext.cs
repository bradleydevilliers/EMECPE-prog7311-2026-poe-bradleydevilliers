using Microsoft.EntityFrameworkCore;
using TechMoveGLMS. Shared.Models.Entities;
//(Microsoft,2026)
namespace TechMoveGLMS.Shared.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        { 
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships using Fluent API
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(s => s.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
// Microsoft, 2026. DbContext Lifetime, Configuration, and Initialization.[Online] Available at: 
//https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/