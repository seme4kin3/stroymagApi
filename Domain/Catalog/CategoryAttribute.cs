
namespace Domain.Catalog
{
    /// <summary>
    /// Связка между категорией и глобальным атрибутом.
    /// </summary>
    public class CategoryAttribute
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid CategoryId { get; private set; }
        public Guid AttributeDefinitionId { get; private set; }
        public Guid? UnitId { get; private set; }

        public bool IsRequired { get; private set; }
        public int SortOrder { get; private set; }
        public virtual Category Category { get; private set; } = default!;
        public virtual AttributeDefinition AttributeDefinition { get; private set; } = default!;
        public virtual MeasurementUnit? Unit { get; private set; }

        public CategoryAttribute(
            Guid categoryId,
            Guid attributeDefinitionId,
            Guid? unitId,
            bool isRequired,
            int sortOrder)
        {
            CategoryId = categoryId;
            AttributeDefinitionId = attributeDefinitionId;
            UnitId = unitId;
            IsRequired = isRequired;
            SortOrder = sortOrder;
        }

        public void Update(Guid? unitId, bool? isRequired, int? sortOrder)
        {
            if (unitId.HasValue)
                UnitId = unitId.Value;

            if (isRequired.HasValue)
                IsRequired = isRequired.Value;

            if (sortOrder.HasValue)
                SortOrder = sortOrder.Value;
        }
    }
}
