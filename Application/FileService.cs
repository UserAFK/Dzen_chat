
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class FileService(IAppDbContext context, IBackgroundTaskQueue queue)
{
    public void QueueFile(Guid commentId, IFormFile file)
    {
        if (file.ContentType == "text/plain" && file.Length > 100 * 1024)
        {
            throw new InvalidOperationException("Text file size exceeds 100KB");
        }

        using var ms = new MemoryStream();
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();

        queue.EnqueueFile(new FileProcessingItem(commentId, file.ContentType, fileBytes));
    }

    public async Task<(string filetype, byte[] filedata, string filename)> GetFileAsync(Guid commentId)
    {
        var fileData = await context.Comments
            .Where(c => c.Id == commentId)
            .Select(c => new { c.FileType, c.FileData })
            .FirstOrDefaultAsync();

        if (fileData == null) return (string.Empty, Array.Empty<byte>(), string.Empty);

        string extension = fileData.FileType switch
        {
            "image/png" => ".png",
            "image/jpeg" => ".jpg",
            "image/gif" => ".gif",
            "text/plain" => ".txt",
            _ => ""
        };

        string fileName = $"comment_{commentId}{extension}";

        return (fileData.FileType, fileData.FileData, fileName);
    }
}
