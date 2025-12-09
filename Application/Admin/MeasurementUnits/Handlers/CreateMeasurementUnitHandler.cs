using Application.Abstractions.Admin;
using MediatR;
using Application.Admin.MeasurementUnits.Commands;
using Domain.Catalog;

namespace Application.Admin.MeasurementUnits.Handlers
{
    public sealed class CreateMeasurementUnitHandler
        : IRequestHandler<CreateMeasurementUnitCommand, Guid>
    {
        private readonly IMeasurementUnitAdminRepository _repo;

        public CreateMeasurementUnitHandler(IMeasurementUnitAdminRepository repo)
            => _repo = repo;

        public async Task<Guid> Handle(CreateMeasurementUnitCommand request, CancellationToken ct)
        {
            var unit = new MeasurementUnit(request.Name, request.Symbol);

            await _repo.AddAsync(unit, ct);
            await _repo.SaveChangesAsync(ct);

            return unit.Id;
        }
    }
}
