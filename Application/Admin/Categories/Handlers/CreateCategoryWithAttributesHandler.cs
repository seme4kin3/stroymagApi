using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class CreateCategoryWithAttributesHandler(
       ICategoryAdminRepository categoryRepo,
       IAttributeAdminRepository attributeRepo
   ) : IRequestHandler<CreateCategoryWithAttributesCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCategoryWithAttributesCommand request, CancellationToken ct)
        {
            // 1. загрузить все нужные AttributeDefinition
            var attrIds = request.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = await attributeRepo.GetByIdsAsync(attrIds, ct);

            if (defs.Count != attrIds.Length)
            {
                var missing = attrIds.Where(id => !defs.ContainsKey(id));
                throw new KeyNotFoundException(
                    $"Не найдены AttributeDefinition: {string.Join(", ", missing)}");
            }

            // 2. создать категорию через доменный конструктор
            var category = new Category(
                name: request.Name,
                parentId: request.ParentId,
                slug: request.Slug,
                imageUrl: request.ImageUrl
            );

            // 3. привязать атрибуты через AttachAttribute
            foreach (var item in request.Attributes.OrderBy(a => a.SortOrder))
            {
                var def = defs[item.AttributeDefinitionId];
                category.AttachAttribute(def, item.IsRequired, item.SortOrder);
            }

            await categoryRepo.AddAsync(category, ct);
            await categoryRepo.SaveChangesAsync(ct);

            return category.Id;
        }
    }
}
