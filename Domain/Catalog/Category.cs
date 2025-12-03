using System.Text;

namespace Domain.Catalog
{
    public class Category
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }

        public Guid? ParentId { get; private set; }

        public string? Slug { get; private set; }
        public string? ImageUrl { get; private set; }
        public virtual Category? Parent { get; private set; }
        public virtual ICollection<Category> Children { get; private set; } = new List<Category>();

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; private set; } = new List<CategoryAttribute>();
        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

        private Category() { }

        public Category(string name, Guid? parentId = null, string? slug = null, string? imageUrl = null)
        {
            SetName(name);
            ParentId = parentId;
            SetSlug(slug ?? GenerateSlugFrom(name));
            SetImage(imageUrl);
        }

        public void Rename(string name)
        {
            SetName(name);

            if (string.IsNullOrWhiteSpace(Slug))
                SetSlug(GenerateSlugFrom(name));
        }

        public void ChangeParent(Guid? parentId) => ParentId = parentId;

        public void SetSlug(string? slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                Slug = null;
                return;
            }
            var trimmed = slug.Trim().ToLowerInvariant();
            if (trimmed.Length > 200)
                trimmed = trimmed[..200];
            Slug = trimmed;
        }

        public void SetImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ImageUrl = null;
                return;
            }
            var trimmed = imageUrl.Trim();
            if (trimmed.Length > 500)
                trimmed = trimmed[..500];
            ImageUrl = trimmed;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required", nameof(name));
            if (name.Length > 200)
                name = name[..200];
            Name = name;
        }

        /// <summary>
        /// Привязать глобальный атрибут к категории.
        /// </summary>
        public CategoryAttribute AttachAttribute(
             AttributeDefinition definition,
             MeasurementUnit? unit,
             bool isRequired = false,
             int sortOrder = 0)
        {
            if (!definition.IsActive)
                throw new InvalidOperationException("Cannot attach inactive attribute.");

            if (CategoryAttributes.Any(a => a.AttributeDefinitionId == definition.Id))
                throw new InvalidOperationException("Attribute already attached to this category.");

            var link = new CategoryAttribute(
                categoryId: Id,
                attributeDefinitionId: definition.Id,
                unitId: unit?.Id,
                isRequired: isRequired,
                sortOrder: sortOrder
            );

            CategoryAttributes.Add(link);
            return link;
        }

        public void UpdateAttachedAttribute(
            Guid attributeDefinitionId,
            Guid? unitId = null,
            bool? isRequired = null,
            int? sortOrder = null)
        {
            var link = CategoryAttributes.SingleOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId)
                ?? throw new InvalidOperationException("Attribute is not attached to this category.");

            link.Update(unitId, isRequired, sortOrder);
        }

        public void DetachAttribute(Guid attributeDefinitionId)
        {
            var link = CategoryAttributes.SingleOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId);
            if (link is not null)
                CategoryAttributes.Remove(link);
        }

        private static string GenerateSlugFrom(string name)
        {
            // примитивный транслит/слаг
            var slug = name.Trim().ToLowerInvariant();


            slug = slug.Replace(' ', '-');


            slug = slug.Replace("ё", "e");


            var sb = new StringBuilder();
            foreach (var ch in slug)
            {
                if ((ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9') ||
                    ch == '-')
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append('-');
                }
            }

            var res = sb.ToString().Trim('-');
            return string.IsNullOrWhiteSpace(res) ? "category" : res;
        }
    }
}
