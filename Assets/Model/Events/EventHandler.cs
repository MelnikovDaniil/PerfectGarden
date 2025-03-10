﻿using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EventHandler : MonoBehaviour
{
    public HandlingStatus Status { get; protected set; }

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

    public virtual void Clear()
    {

    }

    protected async Task PlayAnimationForTheEndAsync(Animator animator, string animationName)
    {
        animator.Play(animationName, 0, 0);
        await Task.Yield();
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            await Task.Yield();
        }
    }
}
