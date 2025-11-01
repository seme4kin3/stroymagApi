using Application.Abstractions;
using Application.Categories.Queries;
using MediatR;


namespace Application.Categories.Handlers
{
    public sealed class GetCategoriesTreeHandler(ICategoryRepository repo)
        : IRequestHandler<GetCategoriesTreeQuery, CategoryTreeListDto>
    {
        public async Task<CategoryTreeListDto> Handle(GetCategoriesTreeQuery request, CancellationToken ct)
        {
            var items = await repo.GetTreeAsync(ct);
            return new CategoryTreeListDto(items);
        }
    }
}
