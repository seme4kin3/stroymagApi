using Application.Common;
using MediatR;


namespace Application.Admin.MeasurementUnits.Queries
{
    public sealed record GetMeasurementUnitsPagedQuery(
        int Page = 1,
        int PageSize = 50,
        string? Name = null,
        string? Symbol = null)
        : IRequest<PagedResult<MeasurementUnitListItemDto>>;
}
