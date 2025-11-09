namespace Application.Dto;

public class CommentReplyDto
{
    public Guid? Id { get; set; }
    public string? Content { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? ParentCommentId { get; set; }
}
