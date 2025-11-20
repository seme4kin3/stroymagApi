using Application.Abstractions;
using Application.Products.DTOs;
using Application.Products.Queries;
using MediatR;


namespace Application.Products.Handlers
{
    //public sealed class GetProductDetailsHandler(IProductReadRepository repo)
    //    : IRequestHandler<GetProductDetailsQuery, ProductDetailsDto>
    //{
    //    public async Task<ProductDetailsDto> Handle(GetProductDetailsQuery request, CancellationToken ct)
    //    {
    //        //var dto = await repo.GetDetailsAsync(request.Id, ct);
    //        //if (dto is null) throw new KeyNotFoundException("Товар не найден");
    //        //return dto;


    //        var dto = new ProductDetailsDto();
    //        return dto;
    //    }
    //}


}
