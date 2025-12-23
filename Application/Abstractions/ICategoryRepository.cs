using Application.Categories;
using Domain.Catalog;

namespace Application.Abstractions
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<CategoryDto>> GetRootCategoriesAsync(CancellationToken ct);
        Task<IReadOnlyList<CategoryTreeDto>> GetTreeAsync(CancellationToken ct);
        Task<IReadOnlyList<(Guid Id, Guid? ParentId, string Slug)>> GetFlatAsync(CancellationToken ct);
    }
}
