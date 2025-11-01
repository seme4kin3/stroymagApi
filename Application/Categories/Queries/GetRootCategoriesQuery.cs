using MediatR;

namespace Application.Categories.Queries
{
    public sealed record GetRootCategoriesQuery() : IRequest<CategoryListDto>;

    public sealed record CategoryListDto(IReadOnlyList<CategoryDto> Items);
}
