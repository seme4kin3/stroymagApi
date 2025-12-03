using MediatR;


namespace Application.Admin.MeasurementUnit.Commands
{
    public sealed record CreateMeasurementUnitCommand(
        string Name,
        string Symbol
    ) : IRequest<Guid>;
}
