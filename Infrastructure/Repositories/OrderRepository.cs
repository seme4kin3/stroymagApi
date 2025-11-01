using Application.Abstractions;
using Domain.Sales;

namespace Infrastructure.Repositories
{
    public class OrderRepository(StroymagDbContext db) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken ct) =>
            await db.Set<Order>().AddAsync(order, ct);

        public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
    }
}
