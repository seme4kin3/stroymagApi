namespace Application.Admin.Products
{
    public sealed record ProductAdminImageDto(
        Guid Id,
        string Url,
        bool IsPrimary,
        int SortOrder
    );
}
