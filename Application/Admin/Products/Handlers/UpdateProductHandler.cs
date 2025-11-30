using Application.Abstractions.Admin;
using Application.Admin.Products.Commands;
using MediatR;

namespace Application.Admin.Products.Handlers
{
    public sealed class UpdateProductHandler(
        IProductAdminRepository productRepo,
        ICategoryAdminRepository categoryRepo,
        IAttributeAdminRepository attributeRepo
    ) : IRequestHandler<UpdateProductCommand>
    {
        public async Task Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var product = await productRepo.GetWithAttributesAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Product not found");

            product.UpdateBasic(
                name: request.Name,
                description: request.Description,
                price: request.Price,
                rrp: request.RecommendedRetailPrice,
                hasStock: request.HasStock
            );

            if (!string.IsNullOrWhiteSpace(request.Article) &&
                request.Article!.Trim() != product.Article)
            {
                product.SetArticle(request.Article);
            }

            product.SetAdvantages(request.Advantages ?? Array.Empty<string>());
            product.SetComplectation(request.Complectation ?? Array.Empty<string>());


            var category = await categoryRepo.GetWithAttributesAsync(product.CategoryId, ct)
                ?? throw new InvalidOperationException("Product category not found");

            var attachedAttrIds = category.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var attrDefs = await attributeRepo.GetByIdsAsync(attachedAttrIds, ct);

            if (attrDefs.Count != attachedAttrIds.Length)
            {
                var missing = attachedAttrIds.Where(id => !attrDefs.ContainsKey(id));
                throw new InvalidOperationException(
                    $"Не найдены определения атрибутов: {string.Join(", ", missing)}");
            }

            var values = request.AttributeValues ?? new Dictionary<Guid, string?>();

            product.ApplyAttributeValues(
                category: category,
                values: values,
                attributeDefinitions: attrDefs
            );

            await productRepo.SaveChangesAsync(ct);
        }
    }
}
