
namespace Application.Categories
{
    public sealed record CategoryTreeDto(
        Guid Id,
        string Name,
        string Slug,
        IReadOnlyList<CategoryTreeDto> Children
    );
}
