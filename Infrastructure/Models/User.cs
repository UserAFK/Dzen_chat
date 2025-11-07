using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class User
{
    public Guid Id { get; set; }
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    public string? HomePage { get; set; }
}
