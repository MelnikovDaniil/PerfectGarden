using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EventHandler<TEvent, TContext> : MonoBehaviour
    where TEvent : Enum
    where TContext : class
{
    public abstract TEvent EventName { get; }
    public HandlingStatus Status { get; protected set; }
    protected TContext Context { get; private set; }

    public void Setup(TContext context)
    {
        Status = HandlingStatus.Scheduled;
        Context = context;
    }

    public async Task PrepareAsync(CancellationToken token = default)
    {
        Status = HandlingStatus.Preparation;
        await PrepareHandlingAsync(token);
    }

    protected virtual async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        await Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken token = default)
    {
        Status = HandlingStatus.Started;
        await StartHandlingAsync(token);

        if (Status != HandlingStatus.Cancelled)
        {
            Status = HandlingStatus.Finished;
        }
    }

    protected virtual async Task StartHandlingAsync(CancellationToken token = default)
    {
        await Task.CompletedTask;
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

    public virtual void Clear()
    {

    }
}
