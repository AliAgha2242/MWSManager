using Dapper;
using MWSMan.Core.Infrastructure.LogManager.LogManager;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;

namespace MWSMan.Core.Infrastructure.LogManager.LogManager
{
    public class LogBackgroundService
    {
        private readonly ILogQueue _queue;
        //private readonly ILogger<LogBackgroundService> _logger;
        private readonly CSConfig _cSConfig;
        private readonly SqlConnectionManager sqlConnectionManager;
        public LogBackgroundService(ILogQueue queue,CSConfig cSConfig)
        {
            _queue = queue;
            _cSConfig = cSConfig;
            sqlConnectionManager = new SqlConnectionManager(cSConfig);
        }

        protected  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var entry) && entry != null)
                    {
                       
                        await sqlConnectionManager.Connection.ExecuteAsync(entry.QueryOrCommand);
                    }
                    else
                    {
                        await Task.Delay(200, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
