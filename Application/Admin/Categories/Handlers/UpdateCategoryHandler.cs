using Application.Abstractions.Admin;
using Application.Admin.Categories.Commands;
using Domain.Catalog;
using MediatR;


namespace Application.Admin.Categories.Handlers
{
    public sealed class UpdateCategoryHandler(
        ICategoryAdminRepository categoryRepo,
        IAttributeAdminRepository attributeRepo
    ) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await categoryRepo.GetWithAttributesAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Категория не найдена");

            // 1. Обновляем базовые поля через доменные методы
            category.Rename(request.Name);
            category.ChangeParent(request.ParentId);
            category.SetSlug(request.Slug);
            category.SetImage(request.ImageUrl);

            // 2. Загружаем определения всех атрибутов из запроса
            var incomingAttrIds = request.Attributes
                .Select(a => a.AttributeDefinitionId)
                .Distinct()
                .ToArray();

            var defs = await attributeRepo.GetByIdsAsync(incomingAttrIds, ct);

            if (defs.Count != incomingAttrIds.Length)
            {
                var missing = incomingAttrIds.Where(id => !defs.ContainsKey(id));
                throw new KeyNotFoundException(
                    $"Не найдены AttributeDefinition: {string.Join(", ", missing)}");
            }

            // 3. Синхронизируем привязки атрибутов

            // 3.1. удаляем те, которых больше нет в новом списке
            var incomingSet = incomingAttrIds.ToHashSet();
            var currentLinks = category.Attributes.ToList(); // чтобы не модифицировать коллекцию в foreach

            foreach (var link in currentLinks)
            {
                if (!incomingSet.Contains(link.AttributeDefinitionId))
                {
                    category.DetachAttribute(link.AttributeDefinitionId);
                }
            }

            // 3.2. добавляем/обновляем те, что есть в команде
            foreach (var item in request.Attributes)
            {
                var def = defs[item.AttributeDefinitionId];

                var existingLink = category.Attributes
                    .FirstOrDefault(a => a.AttributeDefinitionId == def.Id);

                if (existingLink is null)
                {
                    // новая привязка
                    category.AttachAttribute(def, item.IsRequired, item.SortOrder);
                }
                else
                {
                    // обновляем настройки привязки
                    category.UpdateAttachedAttribute(
                        attributeDefinitionId: def.Id,
                        isRequired: item.IsRequired,
                        sortOrder: item.SortOrder
                    );
                }
            }

            await categoryRepo.SaveChangesAsync(ct);
        }
    }
}
