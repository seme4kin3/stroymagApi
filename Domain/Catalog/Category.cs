using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Catalog
{
    public class Category
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }

        public Guid? ParentId { get; private set; }

        public string? Slug { get; private set; }

        // ✅ Храним только идентификатор объекта в storage
        public string? ImageBucket { get; private set; }
        public string? ImageObjectKey { get; private set; }

        public virtual Category? Parent { get; private set; }
        public virtual ICollection<Category> Children { get; private set; } = new List<Category>();

        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; private set; } = new List<CategoryAttribute>();
        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

        private Category() { }

        public Category(string name, Guid? parentId = null, string? slug = null)
        {
            SetName(name);
            ParentId = parentId;
            SetSlug(GenerateSlugFrom(name));
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

        public void SetImage(string bucket, string objectKey)
        {
            if (string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentException("Bucket is required", nameof(bucket));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("ObjectKey is required", nameof(objectKey));

            bucket = bucket.Trim();
            objectKey = objectKey.Trim();

            if (bucket.Length > 100)
                bucket = bucket[..100];

            if (objectKey.Length > 700)
                objectKey = objectKey[..700];

            ImageBucket = bucket;
            ImageObjectKey = objectKey;
        }

        public void ClearImage()
        {
            ImageBucket = null;
            ImageObjectKey = null;
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
            if (string.IsNullOrWhiteSpace(name))
                return "category";

            var slug = Transliterate(name.Trim().ToLowerInvariant());

            var sb = new StringBuilder();
            foreach (var ch in slug)
            {
                if ((ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9'))
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append('-');
                }
            }

            var res = Regex.Replace(sb.ToString(), "-{2,}", "-")
                           .Trim('-');

            return string.IsNullOrWhiteSpace(res) ? "category" : res;
        }

        private static string Transliterate(string input)
        {
            var map = new Dictionary<char, string>
            {
                ['а'] = "a",
                ['б'] = "b",
                ['в'] = "v",
                ['г'] = "g",
                ['д'] = "d",
                ['е'] = "e",
                ['ё'] = "e",
                ['ж'] = "zh",
                ['з'] = "z",
                ['и'] = "i",
                ['й'] = "y",
                ['к'] = "k",
                ['л'] = "l",
                ['м'] = "m",
                ['н'] = "n",
                ['о'] = "o",
                ['п'] = "p",
                ['р'] = "r",
                ['с'] = "s",
                ['т'] = "t",
                ['у'] = "u",
                ['ф'] = "f",
                ['х'] = "h",
                ['ц'] = "ts",
                ['ч'] = "ch",
                ['ш'] = "sh",
                ['щ'] = "sch",
                ['ъ'] = "",
                ['ы'] = "y",
                ['ь'] = "",
                ['э'] = "e",
                ['ю'] = "yu",
                ['я'] = "ya"
            };

            var sb = new StringBuilder();

            foreach (var ch in input)
            {
                if (map.TryGetValue(ch, out var val))
                    sb.Append(val);
                else
                    sb.Append(ch);
            }

            return sb.ToString();
        }
    }
}
