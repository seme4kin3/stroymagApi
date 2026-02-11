using MediatR;

namespace Application.Admin.Products.Commands
{
    public sealed record UploadProductImagesCommand(
        Guid ProductId,
        IReadOnlyList<UploadFileDto> Files
    ) : IRequest;
}
