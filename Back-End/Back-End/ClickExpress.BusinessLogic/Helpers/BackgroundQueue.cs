using System.Threading.Channels;

namespace ClickExpress.BusinessLogic.Helpers
{
    public class BackgroundQueue : IBackgroundQueue
    {
        private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _channel;

        public BackgroundQueue(int capacity = 512)
        {
            _channel = Channel.CreateBounded<Func<IServiceProvider, CancellationToken, Task>>(
                new BoundedChannelOptions(capacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false,
                });
        }

        public void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            _channel.Writer.TryWrite(workItem);
        }

        public async Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken ct)
        {
            return await _channel.Reader.ReadAsync(ct);
        }
    }
}
