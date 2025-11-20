using Application.Abstractions.Admin;
using Application.Admin.Attributes.Commands;
using MediatR;

namespace Application.Admin.Attributes.Handlers
{
    public sealed class UpdateAttributeDefinitionHandler(
        IAttributeAdminRepository repo
    ) : IRequestHandler<UpdateAttributeDefinitionCommand>
    {
        public async Task Handle(UpdateAttributeDefinitionCommand request, CancellationToken ct)
        {
            var attr = await repo.GetAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("AttributeDefinition not found");

            // доменные методы
            attr.Rename(request.Name);
            attr.ChangeKey(request.Key);
            attr.ChangeType(request.DataType);
            attr.ChangeUnit(request.Unit);

            if (request.IsActive)
                attr.Activate();
            else
                attr.Deactivate();

            await repo.SaveChangesAsync(ct);
        }
    }
}
