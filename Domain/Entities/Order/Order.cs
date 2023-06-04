using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order : EntityBase<Guid>
    {
        public Guid Id { get; set; }
        public int RequestedAmount { get; set; }
        public int TotalFoundAmount { get; set; }
        public ProductCrawlType ProductCrawlType { get; set; }
        public ICollection<OrderEvent> OrderEvents { get; set; } //An order can have multiple order events
        public ICollection<Product> Products { get; set; } //An order can have multiple products
    }
}