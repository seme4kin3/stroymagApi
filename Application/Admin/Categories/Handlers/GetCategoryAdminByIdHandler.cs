using Application.Abstractions.Admin;
using Application.Admin.Categories.DTOs;
using Application.Admin.Categories.Queries;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class GetCategoryAdminByIdHandler(
       ICategoryAdminRepository categories,
       IAttributeAdminRepository attributes,
       IMeasurementUnitAdminRepository units,
       IStorageUploader storageUploader
   ) : IRequestHandler<GetCategoryAdminByIdQuery, CategoryAdminDetailsDto?>
    {
        public async Task<CategoryAdminDetailsDto?> Handle(GetCategoryAdminByIdQuery request, CancellationToken ct)
        {
            // 1) Забираем все категории плоско (с CategoryAttributes)
            var flat = await categories.GetFlatWithAttributesAsync(ct);

            // Быстрый доступ по Id
            var byId = flat.ToDictionary(x => x.Id);

            if (!byId.TryGetValue(request.Id, out var target))
                return null;

            // 2) Родитель (Id + Name)
            CategoryParentDto? parentDto = null;
            if (target.ParentId.HasValue && byId.TryGetValue(target.ParentId.Value, out var parent))
            {
                parentDto = new CategoryParentDto(parent.Id, parent.Name);
            }

            // 3) Атрибуты текущей категории (прямые)
            var currentLinks = target.CategoryAttributes
                .OrderBy(a => a.SortOrder)
                .ToList();

            // 4) Собираем атрибуты всех предков (ближайший предок имеет приоритет)
            var inheritedByAttrId = new Dictionary<Guid, CategoryAttribute>();

            var currentParentId = target.ParentId;
            var guard = 0;

            while (currentParentId.HasValue && guard++ < 256)
            {
                if (!byId.TryGetValue(currentParentId.Value, out var curParent))
                    break;

                foreach (var link in curParent.CategoryAttributes.OrderBy(a => a.SortOrder))
                {
                    if (!inheritedByAttrId.ContainsKey(link.AttributeDefinitionId))
                        inheritedByAttrId.Add(link.AttributeDefinitionId, link);
                }

                currentParentId = curParent.ParentId;
            }

            var ancestorAttrIds = inheritedByAttrId.Keys.ToHashSet();

            // 5) Если атрибут есть у предков и у текущей категории —
            // оставляем его в inherited, но с настройками текущей категории
            foreach (var link in currentLinks)
            {
                if (ancestorAttrIds.Contains(link.AttributeDefinitionId))
                    inheritedByAttrId[link.AttributeDefinitionId] = link;
            }

            // 6) Делим own/inherited
            var ownLinks = currentLinks
                .Where(l => !ancestorAttrIds.Contains(l.AttributeDefinitionId))
                .OrderBy(l => l.SortOrder)
                .ToList();

            var inheritedLinks = inheritedByAttrId.Values
                .OrderBy(l => l.SortOrder)
                .ToList();

            // 7) Подтягиваем AttributeDefinition и MeasurementUnit
            var allAttrIds = ownLinks
                .Concat(inheritedLinks)
                .Select(x => x.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = allAttrIds.Length == 0
                ? new Dictionary<Guid, AttributeDefinition>()
                : await attributes.GetByIdsAsync(allAttrIds, ct);

            var unitIds = ownLinks
                .Concat(inheritedLinks)
                .Select(x => x.UnitId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToArray();

            var unitDict = unitIds.Length == 0
                ? new Dictionary<Guid, MeasurementUnit>()
                : await units.GetByIdsAsync(unitIds, ct);

            // 8) Маппинг
            IReadOnlyList<CategoryAttributeResolvedDto> Map(IEnumerable<CategoryAttribute> links) =>
                links.Select(link =>
                {
                    defs.TryGetValue(link.AttributeDefinitionId, out var def);

                    MeasurementUnit? unit = null;
                    if (link.UnitId.HasValue)
                        unitDict.TryGetValue(link.UnitId.Value, out unit);

                    return new CategoryAttributeResolvedDto(
                        AttributeDefinitionId: link.AttributeDefinitionId,
                        AttributeName: def?.Name ?? string.Empty,
                        AttributeKey: def?.Key ?? string.Empty,
                        DataType: def?.DataType ?? default,
                        UnitId: link.UnitId,
                        UnitName: unit?.Name,
                        UnitSymbol: unit?.Symbol,
                        IsRequired: link.IsRequired,
                        SortOrder: link.SortOrder
                    );
                }).ToList();

            var ownDtos = Map(ownLinks);
            var inheritedDtos = Map(inheritedLinks);

            // 9) ImageUrl (строим URL на read-side)
            string? imageUrl = null;
            if (!string.IsNullOrWhiteSpace(target.ImageBucket) &&
                !string.IsNullOrWhiteSpace(target.ImageObjectKey))
            {
                imageUrl = storageUploader.GetPublicUrl(target.ImageBucket!, target.ImageObjectKey!);
            }

            return new CategoryAdminDetailsDto(
                Id: target.Id,
                Name: target.Name,
                Slug: target.Slug,
                ParentId: target.ParentId,
                ImageUrl: imageUrl,              
                Parent: parentDto,
                OwnAttributes: ownDtos,
                InheritedAttributes: inheritedDtos
            );
        }
    }
}
