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
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ File processing failed: {ex.Message}");
                }
            }

            await Task.Delay(500, stoppingToken); // невелика пауза
        }
    }

    private Task<byte[]> ProcessImageAsync(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var image = Image.FromStream(ms);

        if (image.Width <= 320 && image.Height <= 240)
            return Task.FromResult(bytes);

        double ratio = Math.Min(320.0 / image.Width, 240.0 / image.Height);
        int newWidth = (int)(image.Width * ratio);
        int newHeight = (int)(image.Height * ratio);

        using var resized = new Bitmap(image, new Size(newWidth, newHeight));
        using var output = new MemoryStream();
        resized.Save(output, ImageFormat.Png);

        return Task.FromResult(output.ToArray());
    }
}
