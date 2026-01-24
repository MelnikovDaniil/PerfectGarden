using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class StickCareEventHandler : CareEventHandler
{
    public float swipeThreshold = 2f;
    [Space]
    public Stick strickPrefab;
    public Vector2 stickStartPosition;
    public ParticleSystem groundParticlesPrefab;
    public ParticleSystem leavesParticlesPrefab;

    public override CareEvent EventName => CareEvent.Stick;


    private ParticleSystem groundParticles;
    private ParticleSystem leavesParticles;

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

        if (!groundParticles)
        {
            groundParticles = Instantiate(groundParticlesPrefab);
            groundParticles.transform.position = Context.PotWithPlant.dirtCollider.transform.position;
            leavesParticles = Instantiate(leavesParticlesPrefab);
            leavesParticles.transform.position = Context.PotWithPlant.plantRenderer.transform.position;
        }
        createdStick = Instantiate(strickPrefab, Context.PotWithPlant.gameObject.transform);
        createdStick.transform.localPosition = stickStartPosition;

        return Task.CompletedTask;
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        _ = TutorialManager.Instance.SetSwipeAsync(createdStick.gameObject, Vector2.down, swipeThreshold, false, token);
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

        groundParticles.Play();
        leavesParticles.Play();
        await AnimatorHelper.PlayAnimationForTheEndAsync(createdStick.animator, "Stick_Install");
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
