using Application.Abstractions.Admin;
using Application.Admin.MeasurementUnits.Commands;
using MediatR;


namespace Application.Admin.MeasurementUnits.Handlers
{
    public sealed class DeleteMeasurementUnitHandler
        : IRequestHandler<DeleteMeasurementUnitCommand>
    {
        private readonly IMeasurementUnitAdminRepository _repo;

        public DeleteMeasurementUnitHandler(IMeasurementUnitAdminRepository repo)
            => _repo = repo;

        public async Task Handle(DeleteMeasurementUnitCommand request, CancellationToken ct)
        {
            var unit = await _repo.GetAsync(request.Id, ct);
            if (unit is null)
                return;

            unit.Deactivate(); // мягкое удаление

            await _repo.SaveChangesAsync(ct);
        }
    }
}
