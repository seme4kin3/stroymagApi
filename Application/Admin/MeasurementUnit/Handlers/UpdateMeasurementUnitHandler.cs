using Application.Abstractions.Admin;
using Application.Admin.MeasurementUnit.Commands;
using MediatR;

namespace Application.Admin.MeasurementUnit.Handlers
{
    public sealed class UpdateMeasurementUnitHandler
        : IRequestHandler<UpdateMeasurementUnitCommand>
    {
        private readonly IMeasurementUnitAdminRepository _repo;

        public UpdateMeasurementUnitHandler(IMeasurementUnitAdminRepository repo)
            => _repo = repo;

        public async Task Handle(UpdateMeasurementUnitCommand request, CancellationToken ct)
        {
            var unit = await _repo.GetAsync(request.Id, ct)
                ?? throw new KeyNotFoundException($"MeasurementUnit {request.Id} not found");

            unit.Rename(request.Name);
            unit.ChangeSymbol(request.Symbol);

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    unit.Activate();
                else
                    unit.Deactivate();
            }

            await _repo.SaveChangesAsync(ct);
        }
    }
}
