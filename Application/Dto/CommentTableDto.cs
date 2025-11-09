namespace Application.Dto;

public class CommentTableDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? FileType { get; set; }
}
