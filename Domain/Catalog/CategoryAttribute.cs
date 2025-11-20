
namespace Domain.Catalog
{
    /// <summary>
    /// Связка между категорией и глобальным атрибутом.
    /// Здесь мы храним настройки этой пары для UI/валидации.
    /// </summary>
    public class CategoryAttribute
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid CategoryId { get; private set; }
        public Guid AttributeDefinitionId { get; private set; }

        public bool IsRequired { get; private set; }
        public int SortOrder { get; private set; }

        private CategoryAttribute() { }

        public CategoryAttribute(Guid categoryId, Guid attributeDefinitionId, bool isRequired, int sortOrder)
        {
            CategoryId = categoryId;
            AttributeDefinitionId = attributeDefinitionId;
            IsRequired = isRequired;
            SortOrder = sortOrder;
        }

        public void Update(bool? isRequired, int? sortOrder)
        {
            if (isRequired.HasValue)
                IsRequired = isRequired.Value;
            if (sortOrder.HasValue)
                SortOrder = sortOrder.Value;
        }
    }
}
