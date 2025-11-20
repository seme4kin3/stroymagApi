using Application.Common;
using MediatR;


namespace Application.Admin.Categories.Queries
{
    public sealed record GetCategoriesPagedQuery(int Page = 1, int PageSize = 50)
        : IRequest<PagedResult<CategoryListItemDto>>;
}
