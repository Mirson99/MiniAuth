namespace MiniAuth.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null;
    public string PasswordHash { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public DateTime? LastLoginAt { get; set; }
}