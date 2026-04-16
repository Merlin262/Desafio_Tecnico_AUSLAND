namespace ProductsAPI.Application.Auth
{
    /// <summary>
    /// Stores a single hardcoded user for authentication without requiring a database.
    /// Credentials: admin / 123456
    /// </summary>
    public class InMemoryUserStore
    {
        public string Username { get; } = "admin";
        public string PasswordHash { get; } = PasswordHasher.Hash("123456");
    }
}
