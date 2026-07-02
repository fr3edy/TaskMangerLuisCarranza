namespace TaskManager.Application.Features.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; 
    public string Role { get; set; } = "Administrator";
    public string Status { get; set; } = "Active";
}

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = "+1 (555) 019-2837"; 
    public string Role { get; set; } = "System Administrator";
    public string Status { get; set; } = "Active Account";
    public DateTime JoinDate { get; set; }  
    public bool TwoFactorEnabled { get; set; } = true;
}