using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class CreateCategoryHandler(
       ICategoryAdminRepository categoryRepo,
       IAttributeAdminRepository attributeRepo,
       IMeasurementUnitAdminRepository unitRepo
   ) : IRequestHandler<CreateCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            if (request.Attributes is null || request.Attributes.Count == 0)
                throw new InvalidOperationException("Категория должна иметь хотя бы один атрибут.");

            // 1. Тянем AttributeDefinition
            var attrIds = request.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var attrDefs = await attributeRepo.GetByIdsAsync(attrIds, ct);
            if (attrDefs.Count != attrIds.Length)
            {
                var missing = attrIds.Where(id => !attrDefs.ContainsKey(id));
                throw new InvalidOperationException(
                    $"Не найдены AttributeDefinition: {string.Join(", ", missing)}");
            }

            // 2. Тянем MeasurementUnit по UnitId
            var unitIds = request.Attributes
                .Select(a => a.UnitId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToArray();

            var units = unitIds.Length == 0
                ? new Dictionary<Guid, MeasurementUnit>()
                : await unitRepo.GetByIdsAsync(unitIds, ct);

            // 3. Создаём категорию
            var category = new Category(
                name: request.Name,
                parentId: request.ParentId,
                slug: request.Slug,
                imageUrl: request.ImageUrl
            );

            // 4. Привязываем атрибуты
            foreach (var item in request.Attributes.OrderBy(a => a.SortOrder))
            {
                var def = attrDefs[item.AttributeDefinitionId];

                MeasurementUnit? unit = null;
                if (item.UnitId.HasValue)
                    units.TryGetValue(item.UnitId.Value, out unit);

                category.AttachAttribute(
                    definition: def,
                    unit: unit,
                    isRequired: item.IsRequired,
                    sortOrder: item.SortOrder
                );
            }

            await categoryRepo.AddAsync(category, ct);
            await categoryRepo.SaveChangesAsync(ct);

            return category.Id;
        }
    }
}
