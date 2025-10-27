using Infrastructure.Models;

namespace Infrastructure;

public interface IBackgroundTaskQueue
{
    void QueueFile(FileProcessingItem item);
    bool TryDequeue(out FileProcessingItem? item);
}

