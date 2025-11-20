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
        public List<CategoryAttribute> Attributes { get; private set; } = new();
        public ICollection<Product> Products { get; private set; } = new List<Product>();

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
            AttributeDefinition attr,
            bool isRequired = false,
            int sortOrder = 0)
        {
            if (!attr.IsActive)
                throw new InvalidOperationException("Cannot attach inactive attribute.");

            if (Attributes.Any(a => a.AttributeDefinitionId == attr.Id))
                throw new InvalidOperationException("Attribute already attached to this category.");

            var link = new CategoryAttribute(
                categoryId: Id,
                attributeDefinitionId: attr.Id,
                isRequired: isRequired,
                sortOrder: sortOrder
            );

            Attributes.Add(link);
            return link;
        }

        /// <summary>
        /// Обновление настроек привязки атрибута (обязательный, порядок).
        /// </summary>
        public void UpdateAttachedAttribute(Guid attributeDefinitionId, bool? isRequired = null, int? sortOrder = null)
        {
            var link = Attributes.SingleOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId)
                ?? throw new InvalidOperationException("Attribute is not attached to this category.");

            link.Update(isRequired, sortOrder);
        }

        /// <summary>
        /// Открепить атрибут от категории.
        /// (по желанию можно сделать мягкое удаление, но тут просто убираем связь)
        /// </summary>
        public void DetachAttribute(Guid attributeDefinitionId)
        {
            var link = Attributes.SingleOrDefault(a => a.AttributeDefinitionId == attributeDefinitionId);
            if (link is not null)
                Attributes.Remove(link);
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
