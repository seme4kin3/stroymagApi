
namespace Domain.Catalog
{
    public class Category
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }

        public Guid? ParentId { get; private set; }

        // НОВОЕ
        public string? Slug { get; private set; }

        public string? ImageUrl { get; private set; }

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
            // если slug не задавали руками, можем тоже обновить
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

        private static string GenerateSlugFrom(string name)
        {
            // примитивный транслит/слаг — дальше можно заменить на нормальный
            var slug = name.Trim().ToLowerInvariant();

            // заменим пробелы на тире
            slug = slug.Replace(' ', '-');

            // уберём самое очевидное
            slug = slug.Replace("ё", "e");

            // можно оставить только буквы/цифры/дефис
            var sb = new System.Text.StringBuilder();
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
