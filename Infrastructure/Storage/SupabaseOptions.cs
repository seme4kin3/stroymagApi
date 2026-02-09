
namespace Infrastructure.Storage
{
    public sealed class SupabaseOptions
    {
        public required string Url { get; init; }
        public required string ServiceRoleKey { get; init; }
    }
}
