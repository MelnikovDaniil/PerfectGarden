using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class StickCareEventHandler : CareEventHandler
{
    public float swipeThreshold = 2f;
    [Space]
    public Stick strickPrefab;
    public Vector2 stickStartPosition;

    public override CareEvent EventName => CareEvent.Stick;

    private Stick createdStick;

    private Vector2? startTouchPosition;
    private Vector2? endTouchPosition;

    private bool isDetecting = false;
    private bool stickInstalled = false;

    protected override Task PrepareHandlingAsync(CancellationToken token = default)
    {
        isDetecting = false;
        stickInstalled = false;
        startTouchPosition = null;
        endTouchPosition = null;

        createdStick = Instantiate(strickPrefab, Context.PotWithPlant.gameObject.transform);
        createdStick.transform.localPosition = stickStartPosition;

        return Task.CompletedTask;
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        isDetecting = true;

        while (!stickInstalled)
        {
            if (token.IsCancellationRequested)
            {
                await InterruptAsync();
                return;
            }
            await Task.Yield();
        }

        await AnimatorHelper.PlayAnimationForTheEndAsync(createdStick.animator, "Stick_Install");

        await Task.CompletedTask;
    }

    public override void Clear()
    {
        isDetecting = false;
        stickInstalled = false;
        startTouchPosition = null;
        endTouchPosition = null;
        createdStick = null;
    }

    void Update()
    {
        if (isDetecting)
        {
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (startTouchPosition != null && Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipeDown();
        }
    }

    private void DetectSwipeDown()
    {
        var swipeDistance = endTouchPosition.Value.y - startTouchPosition.Value.y;
        Debug.Log($"Swipe distance = {swipeDistance}");
        if (swipeDistance < -swipeThreshold)
        {
            stickInstalled = true;
        }
    }

    protected override async Task InterruptHandlingAsync()
    {
        Destroy(createdStick.gameObject);
        await Task.CompletedTask;
    }
}
