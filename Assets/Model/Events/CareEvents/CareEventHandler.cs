using System.Threading.Tasks;

public abstract class CareEventHandler : EventHandler
{
    public abstract CareEvent EventName { get; }
    protected CareContext Context { get; private set; }

    public void Setup(CareContext context)
    {
        Status = HandlingStatus.Scheduled;
        Context = context;
    }

    public async Task InterruptAsync()
    {
        Status = HandlingStatus.Cancelled;
        await InterruptHandlingAsync();
    }

    protected virtual async Task InterruptHandlingAsync()
    {
        await Task.CompletedTask;
    }
}
