using Application.Abstractions.Admin;
using Application.Admin.Attributes.Commands;
using Domain.Catalog;
using MediatR;

namespace Application.Admin.Attributes.Handlers
{
    public sealed class CreateAttributeDefinitionHandler(IAttributeAdminRepository repo)
        : IRequestHandler<CreateAttributeDefinitionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateAttributeDefinitionCommand request, CancellationToken ct)
        {
            var attr = new AttributeDefinition(request.Name, request.Key, request.DataType, request.Unit);
            await repo.AddAsync(attr, ct);
            await repo.SaveChangesAsync(ct);
            return attr.Id;
        }
    }
}
