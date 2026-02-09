using Application.Abstractions;
using Application.Categories;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal sealed class CategoryRepository(StroymagDbContext db) : ICategoryRepository
    {
        public async Task<IReadOnlyList<CategoryDto>> GetRootCategoriesAsync(CancellationToken ct)
        {
            var roots = await db.Set<Category>()
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.Name)
                .ToListAsync(ct);

            if (roots.Count == 0)
                return Array.Empty<CategoryDto>();

            var rootIds = roots.Select(r => r.Id).ToList();

            var childrenCounts = await db.Set<Category>()
                .Where(c => c.ParentId != null && rootIds.Contains(c.ParentId.Value))
                .GroupBy(c => c.ParentId!.Value)
                .Select(g => new { ParentId = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            var dict = childrenCounts.ToDictionary(x => x.ParentId, x => x.Count);

            var result = roots
                .Select(r => new CategoryDto(
                    r.Id,
                    r.Name,
                    r.Slug,
                    dict.TryGetValue(r.Id, out var cnt) ? cnt : 0
                ))
                .ToList()
                .AsReadOnly();

            return result;
        }

        public async Task<IReadOnlyList<CategoryTreeDto>> GetTreeAsync(CancellationToken ct)
        {
            // 1. все категории
            var all = await db.Set<Category>()
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);

            if (all.Count == 0)
                return Array.Empty<CategoryTreeDto>();

            // 2. корни (parent == null)
            var roots = all.Where(c => c.ParentId == null).ToList();

            // 3. группы только по НЕ null-ParentId
            var byParent = all
                .Where(c => c.ParentId != null)
                .GroupBy(c => c.ParentId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 4. рекурсивная сборка
            List<CategoryTreeDto> BuildChildren(Guid parentId)
            {
                if (!byParent.TryGetValue(parentId, out var children))
                    return new List<CategoryTreeDto>();

                var list = new List<CategoryTreeDto>(children.Count);

                foreach (var ch in children.OrderBy(x => x.Name))
                {
                    list.Add(new CategoryTreeDto(
                        ch.Id,
                        ch.Name,
                        ch.Slug,
                        BuildChildren(ch.Id)
                    ));
                }

                return list;
            }

            // 5. собираем корень
            var result = new List<CategoryTreeDto>(roots.Count);
            foreach (var root in roots.OrderBy(r => r.Name))
            {
                result.Add(new CategoryTreeDto(
                    root.Id,
                    root.Name,
                    root.Slug,
                    BuildChildren(root.Id)
                ));
            }

            return result;
        }

        public async Task<IReadOnlyList<(Guid Id, Guid? ParentId, string Slug)>> GetFlatAsync(CancellationToken ct)
        {
            var items = await db.Set<Category>()
                .AsNoTracking()
                .Select(c => new { c.Id, c.ParentId, c.Slug })
                .ToListAsync(ct);

            return items
                .Select(x => (x.Id, x.ParentId, x.Slug))
                .ToList()
                .AsReadOnly();
        }
    }
}
