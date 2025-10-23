using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class Comment
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    [Timestamp]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
