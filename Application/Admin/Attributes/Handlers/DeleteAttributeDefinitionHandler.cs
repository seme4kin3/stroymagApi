using Application.Abstractions.Admin;
using Application.Admin.Attributes.Commands;
using MediatR;


namespace Application.Admin.Attributes.Handlers
{
    public sealed class DeleteAttributeDefinitionHandler(
        IAttributeAdminRepository repo
    ) : IRequestHandler<DeleteAttributeDefinitionCommand>
    {
        public async Task Handle(DeleteAttributeDefinitionCommand request, CancellationToken ct)
        {
            var attr = await repo.GetAsync(request.Id, ct);
            if (attr is null)
                return; // идемпотентность

            // мягкое удаление
            attr.Deactivate();

            await repo.SaveChangesAsync(ct);
        }
    }
}
