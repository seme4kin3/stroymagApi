using MediatR;


namespace Application.Admin.Products.Queries
{
    public sealed record GetProductAdminByIdQuery(Guid Id)
    : IRequest<ProductAdminListItemDto>;
}
