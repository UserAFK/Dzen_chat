using Microsoft.AspNetCore.Http;

namespace Application.Dto;

public class CommentDto
{
    public Guid? Id { get; set; }
    public string? Content { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? HomePage { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? FileType { get; set; }
    public Guid? ParentCommentId { get; set; }
    public IFormFile? File { get; set; }
    public string Recaptcha { get; set; } = string.Empty;
    public CommentReplyDto? ParentComment { get; set; }
    public List<CommentReplyDto> Replies { get; set; } = new();
}
