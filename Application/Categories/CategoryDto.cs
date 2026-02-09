
namespace Application.Categories
{
    public sealed record CategoryDto(
        Guid Id,
        string Name,
        string Slug,
        int ChildrenCount
    );
}
