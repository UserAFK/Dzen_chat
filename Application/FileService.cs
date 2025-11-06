
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;

namespace Application;

public class FileService(IAppDbContext context)
{
    private const int MaxImageWidth = 320;
    private const int MaxImageHeight = 240;
    public async Task<Comment> CreateAsync(Comment comment, IFormFile? file)
    {
        if (file != null)
        {
            comment.FileType = file.ContentType;

            if (file.ContentType.StartsWith("image/"))
            {
                comment.FileData = await ProcessImageAsync(file);
            }
            else if (file.ContentType == "text/plain")
            {
                if (file.Length > 100 * 1024)
                    throw new InvalidOperationException("Text file size exceeds 100KB");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                comment.FileData = ms.ToArray();
            }
            else
            {
                throw new InvalidOperationException($"File of type {file.ContentType} cannot be saved.");
            }
        }

        return comment;
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

    private async Task<byte[]> ProcessImageAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var image = Image.FromStream(stream);

        int width = image.Width;
        int height = image.Height;
        if (width > MaxImageWidth || height > MaxImageHeight)
        {
            var ratioX = (double)MaxImageWidth / width;
            var ratioY = (double)MaxImageHeight / height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(width * ratio);
            var newHeight = (int)(height * ratio);

            using var resized = new Bitmap(image, new Size(newWidth, newHeight));
            using var ms = new MemoryStream();
            resized.Save(ms, ImageFormat.Png);
            return await Task.FromResult(ms.ToArray());
        }
        else
        {
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return await Task.FromResult(ms.ToArray());
        }
    }
}
