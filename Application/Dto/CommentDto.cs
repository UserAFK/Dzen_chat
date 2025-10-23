namespace Application.Dto;

public class CommentDto
{
    public Guid? Id { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
}
