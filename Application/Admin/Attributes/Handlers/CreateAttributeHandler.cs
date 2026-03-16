using Application.Abstractions.Admin;
using Application.Admin.Attributes.Commands;
using Domain.Catalog;
using MediatR;

namespace Application.Admin.Attributes.Handlers
{
    public sealed class CreateAttributeHandler(IAttributeAdminRepository repo)
        : IRequestHandler<CreateAttributeCommand, Guid>
    {
        public async Task<Guid> Handle(CreateAttributeCommand request, CancellationToken ct)
        {
            var attr = new AttributeDefinition(request.Name, request.DataType);
            await repo.AddAsync(attr, ct);
            await repo.SaveChangesAsync(ct);
            return attr.Id;
        }
    }
}
