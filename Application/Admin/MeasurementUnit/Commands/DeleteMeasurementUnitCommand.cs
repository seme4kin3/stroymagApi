using MediatR;

namespace Application.Admin.MeasurementUnit.Commands
{
    public sealed record DeleteMeasurementUnitCommand(Guid Id) : IRequest;
}
