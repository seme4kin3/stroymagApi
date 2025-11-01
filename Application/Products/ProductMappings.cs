using Domain.Catalog;

namespace Application.Products
{
    internal static class ProductMappings
    {
        public static ProductDto ToDto(this Product p) =>
            new(p.Id, p.Sku, p.Name, p.Article, p.Price);
    }
}
