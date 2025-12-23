using Application.Admin.Categories.DTOs;
using MediatR;

namespace Application.Admin.Categories.Queries
{
    public sealed record GetCategoryAdminByIdQuery(Guid Id) : IRequest<CategoryAdminDetailsDto?>;

}
