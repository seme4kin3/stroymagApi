using Application.Abstractions.Admin;
using Application.Admin.MeasurementUnits.Commands;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Admin.MeasurementUnits.Handlers
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
                ?? throw new NotFoundException("Единица измерения не найдена.");

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
