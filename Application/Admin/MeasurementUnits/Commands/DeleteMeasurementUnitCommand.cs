using MediatR;

namespace Application.Admin.MeasurementUnits.Commands
{
    public sealed record DeleteMeasurementUnitCommand(Guid Id) : IRequest;
}
