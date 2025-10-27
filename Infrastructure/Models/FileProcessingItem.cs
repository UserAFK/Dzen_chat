namespace Infrastructure.Models;

public record FileProcessingItem(
    Guid CommentId,
    string FileType,
    byte[] FileBytes
);
