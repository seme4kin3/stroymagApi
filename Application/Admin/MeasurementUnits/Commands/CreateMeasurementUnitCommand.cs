using MediatR;


namespace Application.Admin.MeasurementUnits.Commands
{
    public sealed record CreateMeasurementUnitCommand(
        string Name,
        string Symbol
    ) : IRequest<Guid>;
}
