using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Application.Common.Exceptions;
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
                ?? throw new NotFoundException("Категория не найдена.");

            if (request.Attributes is null || request.Attributes.Count == 0)
                throw new DomainException("Категория должна иметь хотя бы один атрибут.");

            // 1) базовые поля
            category.Rename(request.Name);
            category.ChangeParent(request.ParentId);
            category.SetSlug(request.Slug);

            // 2) AttributeDefinition
            var incomingAttrIds = request.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var attrDefs = await attributeRepo.GetByIdsAsync(incomingAttrIds, ct);
            if (attrDefs.Count != incomingAttrIds.Length)
            {
                var missing = incomingAttrIds.Where(id => !attrDefs.ContainsKey(id));
                throw new NotFoundException($"Не найдены определения атрибутов: {string.Join(", ", missing)}");
            }

            // 3) Units
            var unitIds = request.Attributes
                .Select(a => a.UnitId)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToArray();

            var units = unitIds.Length == 0
                ? new Dictionary<Guid, MeasurementUnit>()
                : await unitRepo.GetByIdsAsync(unitIds, ct);

            // 4) синхронизация атрибутов
            var existingLinks = category.CategoryAttributes.ToList();
            var incomingSet = incomingAttrIds.ToHashSet();

            // 4.1 удалить лишние
            foreach (var link in existingLinks)
            {
                if (!incomingSet.Contains(link.AttributeDefinitionId))
                    category.DetachAttribute(link.AttributeDefinitionId);
            }

            // 4.2 добавить/обновить
            foreach (var item in request.Attributes)
            {
                var def = attrDefs[item.AttributeDefinitionId];

                var existing = category.CategoryAttributes
                    .FirstOrDefault(a => a.AttributeDefinitionId == def.Id);

                if (existing is null)
                {
                    MeasurementUnit? unit = null;
                    if (item.UnitId.HasValue)
                        units.TryGetValue(item.UnitId.Value, out unit);

                    category.AttachAttribute(def, unit, item.IsRequired, item.SortOrder);
                }
                else
                {
                    existing.Update(item.UnitId, item.IsRequired, item.SortOrder);
                }
            }

            await categoryRepo.SaveChangesAsync(ct);
        }
    }
}
