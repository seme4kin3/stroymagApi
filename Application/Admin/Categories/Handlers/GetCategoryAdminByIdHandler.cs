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
        IMeasurementUnitAdminRepository units
    ) : IRequestHandler<GetCategoryAdminByIdQuery, CategoryAdminDetailsDto?>
    {
        public async Task<CategoryAdminDetailsDto> Handle(
           GetCategoryAdminByIdQuery request,
           CancellationToken ct)
        {
            // 1) Забираем все категории плоско (с CategoryAttributes)
            var flat = await categories.GetFlatWithAttributesAsync(ct);

            var target = flat.FirstOrDefault(c => c.Id == request.Id);
            if (target is null)
                return null;

            // 2) Родитель (Id + Name)
            CategoryParentDto? parentDto = null;
            if (target.ParentId.HasValue)
            {
                var parent = flat.FirstOrDefault(x => x.Id == target.ParentId.Value);
                if (parent is not null)
                    parentDto = new CategoryParentDto(parent.Id, parent.Name);
            }

            // 3) Атрибуты текущей категории (прямые)
            var currentLinks = target.CategoryAttributes
                .OrderBy(a => a.SortOrder)
                .ToList();

            // 4) Собираем атрибуты всех предков
            // key = AttributeDefinitionId
            // value = CategoryAttribute (настройки ближайшего предка)
            var inheritedByAttrId = new Dictionary<Guid, CategoryAttribute>();

            var currentParentId = target.ParentId;
            var guard = 0; // защита от циклов

            while (currentParentId.HasValue && guard++ < 256)
            {
                var parent = flat.FirstOrDefault(x => x.Id == currentParentId.Value);
                if (parent is null)
                    break;

                foreach (var link in parent.CategoryAttributes.OrderBy(a => a.SortOrder))
                {
                    // добавляем только если атрибут ещё не встречался
                    // (ближайший предок имеет приоритет)
                    if (!inheritedByAttrId.ContainsKey(link.AttributeDefinitionId))
                    {
                        inheritedByAttrId.Add(link.AttributeDefinitionId, link);
                    }
                }

                currentParentId = parent.ParentId;
            }

            var ancestorAttrIds = inheritedByAttrId.Keys.ToHashSet();

            // 5) Если атрибут есть и у предков, и у текущей категории —
            // он остаётся inherited, но с настройками текущей категории
            foreach (var link in currentLinks)
            {
                if (ancestorAttrIds.Contains(link.AttributeDefinitionId))
                {
                    inheritedByAttrId[link.AttributeDefinitionId] = link;
                }
            }

            // 6) Делим:
            // Own = только те, которых нет ни у одного предка
            var ownLinks = currentLinks
                .Where(l => !ancestorAttrIds.Contains(l.AttributeDefinitionId))
                .OrderBy(l => l.SortOrder)
                .ToList();

            // Inherited = все, что пришло от предков
            var inheritedLinks = inheritedByAttrId.Values
                .OrderBy(l => l.SortOrder)
                .ToList();

            // 7) Подтягиваем AttributeDefinition и MeasurementUnit
            var allAttrIds = ownLinks
                .Concat(inheritedLinks)
                .Select(x => x.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = await attributes.GetByIdsAsync(allAttrIds, ct);

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

            // 8) Маппинг в DTO
            IReadOnlyList<CategoryAttributeResolvedDto> Map(IEnumerable<CategoryAttribute> links) =>
                links
                    .Select(link =>
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
                    })
                    .ToList();

            var ownDtos = Map(ownLinks);
            var inheritedDtos = Map(inheritedLinks);

            // 9) Финальный DTO
            return new CategoryAdminDetailsDto(
                Id: target.Id,
                Name: target.Name,
                Slug: target.Slug,
                ParentId: target.ParentId,
                ImageUrl: target.ImageUrl,
                Parent: parentDto,
                OwnAttributes: ownDtos,
                InheritedAttributes: inheritedDtos
            );
        }
    }
}
