using Infrastructure.Models;

namespace Infrastructure;

public interface IBackgroundTaskQueue
{
    void EnqueueFile(FileProcessingItem item);
    bool TryDequeue(out FileProcessingItem? item);
}

