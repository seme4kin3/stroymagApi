using Application.Abstractions;
using Application.Categories.Queries;
using MediatR;

namespace Application.Categories.Handlers
{
    public sealed class GetRootCategoriesHandler(ICategoryRepository repo)
        : IRequestHandler<GetRootCategoriesQuery, CategoryListDto>
    {
        public async Task<CategoryListDto> Handle(GetRootCategoriesQuery request, CancellationToken ct)
        {
            var items = await repo.GetRootCategoriesAsync(ct);
            return new CategoryListDto(items);
        }
    }
}
