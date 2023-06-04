using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<OrderEvent> OrderEvents { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken); // Cancellation Token is used to interrupt an asynchronous transaction.
        int SaveChanges();
    }
}