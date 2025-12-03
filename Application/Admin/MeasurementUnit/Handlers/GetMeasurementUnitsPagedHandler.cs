using Application.Abstractions.Admin;
using Application.Admin.MeasurementUnit.Queries;
using Application.Common;
using MediatR;


namespace Application.Admin.MeasurementUnit.Handlers
{
    public sealed class GetMeasurementUnitsPagedHandler
        : IRequestHandler<GetMeasurementUnitsPagedQuery, PagedResult<MeasurementUnitListItemDto>>
    {
        private readonly IMeasurementUnitAdminRepository _repo;

        public GetMeasurementUnitsPagedHandler(IMeasurementUnitAdminRepository repo)
            => _repo = repo;

        public async Task<PagedResult<MeasurementUnitListItemDto>> Handle(
            GetMeasurementUnitsPagedQuery request,
            CancellationToken ct)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 50;

            var (items, total) = await _repo.GetPagedAsync(page, pageSize, ct);

            var dtoItems = items
                .Select(u => new MeasurementUnitListItemDto(
                    u.Id,
                    u.Name,
                    u.Symbol,
                    u.IsActive
                ))
                .ToList();

            return new PagedResult<MeasurementUnitListItemDto>(dtoItems, total, page, pageSize);
        }
    }
}
