
namespace Domain.Sales
{
    public class Customer
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string? Phone { get; private set; }

        private Customer() { }
        public Customer(string fullName, string email, string? phone = null)
        {
            FullName = string.IsNullOrWhiteSpace(fullName) ? throw new ArgumentException("Name required") : fullName.Trim();
            Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email required") : email.Trim().ToLowerInvariant();
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        }
    }
}
