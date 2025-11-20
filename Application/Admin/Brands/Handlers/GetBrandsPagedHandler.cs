using Application.Abstractions.Admin;
using Application.Admin.Brands.Queries;
using Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.Brands.Handlers
{
    public sealed class GetBrandsPagedHandler(IBrandAdminRepository repo)
        : IRequestHandler<GetBrandsPagedQuery, PagedResult<BrandListItemDto>>
    {
        public async Task<PagedResult<BrandListItemDto>> Handle(
            GetBrandsPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page;
            var pageSize = request.PageSize;

            var (items, total) = await repo.GetPagedAsync(page, pageSize, ct);

            var dtoItems = items
                .Select(b => new BrandListItemDto(
                    b.Id,
                    b.Name
                ))
                .ToList();

            return new PagedResult<BrandListItemDto>(dtoItems, total, page, pageSize);
        }
    }
}
