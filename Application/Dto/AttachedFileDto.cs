namespace Application.Dto;

public record AttachedFileDto(
int CommentId,
string FileType,
byte[] FileBytes
);
