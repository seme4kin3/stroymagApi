
using Microsoft.AspNetCore.Http;

namespace Application.Admin.Categories.DTOs
{
    public sealed class CreateCategoryForm
    {
        public string Name { get; set; } = default!;
        public Guid? ParentId { get; set; }
        public string? Slug { get; set; }

        public string AttributesJson { get; set; } = default!; // список атрибутов как JSON строка
        public IFormFile? Image { get; set; }
    }
}
