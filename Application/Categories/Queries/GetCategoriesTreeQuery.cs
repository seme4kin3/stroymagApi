using MediatR;


namespace Application.Categories.Queries
{
    public sealed record GetCategoriesTreeQuery() : IRequest<CategoryTreeListDto>;

    public sealed record CategoryTreeListDto(IReadOnlyList<CategoryTreeDto> Items);
}
