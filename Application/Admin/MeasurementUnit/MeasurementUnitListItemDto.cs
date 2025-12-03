
namespace Application.Admin.MeasurementUnit
{
    public sealed record MeasurementUnitListItemDto(
        Guid Id,
        string Name,
        string Symbol,
        bool IsActive
    );
}
