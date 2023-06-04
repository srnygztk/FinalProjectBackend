using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastracture.Persistence.Contexts;

public class ApplicationDbContext: DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderEvent> OrderEvents { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
    { 
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurations 
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Ignores 
            
        base.OnModelCreating(modelBuilder);
    }
}

