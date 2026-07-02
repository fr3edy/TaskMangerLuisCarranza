namespace TaskManager.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Role { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    private User() { }

    public User(Guid id, string email, string passwordHash, string name, string role = "Administrator", string status = "Active")
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        Role = role;
        Status = status;
    }
}