
namespace Application.Admin.MeasurementUnits
{
    public sealed record MeasurementUnitListItemDto(
        Guid Id,
        string Name,
        string Symbol,
        bool IsActive
    );
}
