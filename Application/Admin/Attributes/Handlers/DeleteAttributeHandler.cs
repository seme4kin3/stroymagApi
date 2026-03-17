using Application.Abstractions.Admin;
using Application.Admin.Attributes.Commands;
using Application.Common.Exceptions;
using MediatR;


namespace Application.Admin.Attributes.Handlers
{
    public sealed class DeleteAttributeHandler
        : IRequestHandler<DeleteAttributeCommand, Unit>
    {
        private readonly IAttributeAdminRepository _repo;

        public DeleteAttributeHandler(IAttributeAdminRepository repo)
            => _repo = repo;

        public async Task<Unit> Handle(DeleteAttributeCommand request, CancellationToken ct)
        {
            var attr = await _repo.GetByIdAsync(request.Id, ct)
                ?? throw new NotFoundException($"Атрибут {request.Id} не найден.");

            // мягкое удаление
            attr.Deactivate();

            await _repo.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
