using Domain.Sales;

namespace Application.Abstractions
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
