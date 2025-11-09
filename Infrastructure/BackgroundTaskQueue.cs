using Infrastructure.Models;
using System.Collections.Concurrent;

namespace Infrastructure;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ConcurrentQueue<FileProcessingItem> _queue = new();

    public void EnqueueFile(FileProcessingItem item)
    {
        _queue.Enqueue(item);
    }

    public bool TryDequeue(out FileProcessingItem? item)
    {
        return _queue.TryDequeue(out item);
    }
}