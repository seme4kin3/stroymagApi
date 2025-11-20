using Application.Products.DTOs;
using MediatR;

namespace Application.Products.Queries
{
    //public sealed record GetProductDetailsQuery(Guid Id) : IRequest<ProductDetailsDto>;


    //заглушка
    public sealed record GetProductDetailsQuery(Guid Id) : IRequest<string>;
}
