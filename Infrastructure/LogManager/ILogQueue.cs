using MWSMan.Core.Model.PublicModels.LogModels;
using System.Collections.Concurrent;

namespace MWSMan.Core.Infrastructure.LogManager.LogManager
{
    public interface ILogQueue
    {
        void Enqueue(BaseLogModel entry);
        bool TryDequeue(out BaseLogModel? entry);
    }
    public class LogQueue : ILogQueue
    {
        private readonly ConcurrentQueue<BaseLogModel> _queue = new();

        public void Enqueue(BaseLogModel entry) => _queue.Enqueue(entry);

        public bool TryDequeue(out BaseLogModel? entry) => _queue.TryDequeue(out entry);
    }
}
