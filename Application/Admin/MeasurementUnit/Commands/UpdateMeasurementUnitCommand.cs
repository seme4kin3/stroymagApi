using MediatR;


namespace Application.Admin.MeasurementUnit.Commands
{
    public sealed record UpdateMeasurementUnitCommand(
        Guid Id,
        string Name,
        string Symbol,
        bool? IsActive = null
    ) : IRequest;
}
