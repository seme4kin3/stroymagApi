using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class UpdateCategoryHandler(
       ICategoryAdminRepository categoryRepo,
       IAttributeAdminRepository attributeRepo,
       IMeasurementUnitAdminRepository unitRepo
   ) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await categoryRepo.GetWithAttributesAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Категория не найдена");

            if (request.Attributes is null || request.Attributes.Count == 0)
                throw new InvalidOperationException("Категория должна иметь хотя бы один атрибут.");

            // 1. Базовые поля
            category.Rename(request.Name);
            category.ChangeParent(request.ParentId);
            category.SetSlug(request.Slug);
            category.SetImage(request.ImageUrl);

            // 2. Тянем AttributeDefinition
            var incomingAttrIds = request.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var attrDefs = await attributeRepo.GetByIdsAsync(incomingAttrIds, ct);
            if (attrDefs.Count != incomingAttrIds.Length)
            {
                var missing = incomingAttrIds.Where(id => !attrDefs.ContainsKey(id));
                throw new InvalidOperationException(
                    $"Не найдены AttributeDefinition: {string.Join(", ", missing)}");
            }

            // 3. Тянем MeasurementUnit по UnitId
            var unitIds = request.Attributes
                .Select(a => a.UnitId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToArray();

            var units = unitIds.Length == 0
                ? new Dictionary<Guid, MeasurementUnit>()
                : await unitRepo.GetByIdsAsync(unitIds, ct);

            // 4. Синхронизируем Category.CategoryAttributes

            var existingLinks = category.CategoryAttributes.ToList();
            var incomingSet = incomingAttrIds.ToHashSet();

            // 4.1. Удаляем те, которых больше нет
            foreach (var link in existingLinks)
            {
                if (!incomingSet.Contains(link.AttributeDefinitionId))
                {
                    category.DetachAttribute(link.AttributeDefinitionId);
                }
            }

            // 4.2. Добавляем/обновляем
            foreach (var item in request.Attributes)
            {
                var def = attrDefs[item.AttributeDefinitionId];

                Guid? unitId = item.UnitId;

                var existing = category.CategoryAttributes
                    .FirstOrDefault(a => a.AttributeDefinitionId == def.Id);

                if (existing is null)
                {
                    // новая связь
                    MeasurementUnit? unit = null;
                    if (unitId.HasValue)
                        units.TryGetValue(unitId.Value, out unit);

                    category.AttachAttribute(def, unit, item.IsRequired, item.SortOrder);
                }
                else
                {
                    // обновление связи: Update(Guid? unitId, bool? isRequired, int? sortOrder)
                    existing.Update(
                        unitId: unitId,
                        isRequired: item.IsRequired,
                        sortOrder: item.SortOrder
                    );
                }
            }

            await categoryRepo.SaveChangesAsync(ct);
        }
    }
}
