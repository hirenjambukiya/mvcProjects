using System;

namespace MSE.StockExchange.Models.Domain;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int FailedAttemptCount { get; set; } = 0;
    public bool IsLockedOut { get; set; } = false;
    public DateTime? LockoutEnd { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // For joining roles
    public string RoleName { get; set; } = string.Empty;
}
