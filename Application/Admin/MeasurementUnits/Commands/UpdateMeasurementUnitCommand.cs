using MediatR;


namespace Application.Admin.MeasurementUnits.Commands
{
    public sealed record UpdateMeasurementUnitCommand(
        Guid Id,
        string Name,
        string Symbol,
        bool? IsActive = null
    ) : IRequest;
}
