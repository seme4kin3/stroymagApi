using System;

namespace Domain.Catalog
{
    public class ProductImage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ProductId { get; private set; } // FK на Product.Id
        public string Url { get; private set; }       // публичный URL (или относительный)
        public string StoragePath { get; private set; } // физический путь/ключ на сервере/в бакете
        public string? Alt { get; private set; }
        public bool IsPrimary { get; private set; }
        public int SortOrder { get; private set; }
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        private ProductImage() { }
        public ProductImage(Guid productId, string url, string storagePath, string? alt, bool isPrimary, int sortOrder)
        {
            ProductId = productId;
            Url = url;
            StoragePath = storagePath;
            Alt = string.IsNullOrWhiteSpace(alt) ? null : alt.Trim();
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
        }

        internal void MakePrimary() => IsPrimary = true;
        internal void ClearPrimary() => IsPrimary = false;
        public void SetSort(int order) => SortOrder = order;
        public void SetAlt(string? alt) => Alt = string.IsNullOrWhiteSpace(alt) ? null : alt.Trim();
    }
}
