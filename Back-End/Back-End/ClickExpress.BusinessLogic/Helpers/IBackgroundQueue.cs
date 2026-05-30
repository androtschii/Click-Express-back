namespace ClickExpress.BusinessLogic.Helpers
{
    public interface IBackgroundQueue
    {
        void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem);
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken ct);
    }
}
