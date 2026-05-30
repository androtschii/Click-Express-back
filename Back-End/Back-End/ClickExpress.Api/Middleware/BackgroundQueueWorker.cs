using ClickExpress.BusinessLogic.Helpers;

namespace ClickExpress.Api.Middleware
{
    public class BackgroundQueueWorker : BackgroundService
    {
        private readonly IBackgroundQueue _queue;
        private readonly IServiceProvider _services;
        private readonly ILogger<BackgroundQueueWorker> _logger;

        public BackgroundQueueWorker(IBackgroundQueue queue, IServiceProvider services, ILogger<BackgroundQueueWorker> logger)
        {
            _queue = queue;
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await _queue.DequeueAsync(stoppingToken);
                    using var scope = _services.CreateScope();
                    await workItem(scope.ServiceProvider, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background queue work item failed");
                }
            }
        }
    }
}
