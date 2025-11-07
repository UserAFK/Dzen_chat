using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Drawing.Imaging;

namespace Infrastructure;

public class FileProcessingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBackgroundTaskQueue _queue;

    public FileProcessingService(IServiceScopeFactory scopeFactory, IBackgroundTaskQueue queue)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var item))
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                try
                {
                    byte[] processedData = item.FileType.StartsWith("image/")
                        ? await ProcessImageAsync(item.FileBytes)
                        : item.FileBytes;

                    var comment = await db.Comments.FindAsync(item.CommentId);
                    if (comment != null)
                    {
                        comment.FileData = processedData;
                        comment.FileType = item.FileType;
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File processing failed: {ex.Message}");
                }
            }

            await Task.Delay(500, stoppingToken);
        }
    }

    private Task<byte[]> ProcessImageAsync(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var image = Image.FromStream(stream);
        int width = image.Width;
        int height = image.Height;
        if (width > 320 || height > 240)
        {
            var ratioX = 320.0 / width;
            var ratioY = 240.0 / height;
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(width * ratio);
            var newHeight = (int)(height * ratio);
            using var resized = new Bitmap(image, new Size(newWidth, newHeight));
            using var output = new MemoryStream();
            resized.Save(output, ImageFormat.Png);
            return Task.FromResult(output.ToArray());
        }
        else
        {
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return Task.FromResult(ms.ToArray());
        }
    }
}
