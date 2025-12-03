using Application.Abstractions.Admin;
using Application.Admin.Attributes.Commands;
using MediatR;

namespace Application.Admin.Attributes.Handlers
{
    public sealed class UpdateAttributeHandler
        : IRequestHandler<UpdateAttributeCommand, Unit>
    {
        private readonly IAttributeAdminRepository _repo;

        public UpdateAttributeHandler(IAttributeAdminRepository repo)
            => _repo = repo;

        public async Task<Unit> Handle(UpdateAttributeCommand request, CancellationToken ct)
        {
            var attr = await _repo.GetByIdAsync(request.Id, ct)
                ?? throw new KeyNotFoundException($"Атрибут {request.Id} не найден.");

            // доменные методы из AttributeDefinition
            attr.Rename(request.Name);
            attr.ChangeKey(request.Key);
            attr.ChangeType(request.DataType);

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    attr.Activate();
                else
                    attr.Deactivate();
            }

            await _repo.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
