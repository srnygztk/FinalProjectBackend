using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public static class ProductFilter
    {
        public static List<Product> FilterBySalePrice(List<Product> products)
        {
            return products.Where(p => p.SalePrice > 0).ToList();
        }

        public static List<Product> FilterByPrice(List<Product> products)
        {
            return products.Where(p => p.SalePrice == 0).ToList();
        }

        public static List<Product> FilterAll(List<Product> products)
        {
            return products;
        }
    }
}
