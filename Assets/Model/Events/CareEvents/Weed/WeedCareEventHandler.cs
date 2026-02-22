using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WeedCareEventHandler : CareEventHandler
{
    public override CareEvent EventName => CareEvent.Weed;
    public float pullThreshold = 5;
    public float cameraZoom = 5;
    public Vector3 cameraOffset;

    private bool pullingOutStarted = false;
    private LayerMask weedMask;

    private WeedCareState state;

    private Weed pulledWeed;

    private void Awake()
    {
        weedMask = LayerMask.GetMask("Weed");
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        state = Context.PotWithPlant.GetState<WeedCareState>();
        CameraManager.Instanse.LookAtPoint(Context.PotWithPlant.dirtCollider.transform.position, cameraZoom, cameraOffset);
        PlantRotationManager.Instance.SetRotationEnabled(true);
        foreach (var weed in state.weeds)
        {
            _ = TutorialManager.Instance.SetSwipeAsync(weed.gameObject, Vector2.up, 1f, false, token);
        }
        pullingOutStarted = true;
        while (state.weedNumberLeft > 0)
        {
            if (token.IsCancellationRequested)
            {
                await InterruptAsync();
                return;
            }
            await Task.Yield();
        }
    }

    private void Update()
    {
        if (pullingOutStarted)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var input = Input.mousePosition;
                var ray = Camera.main.ScreenPointToRay(input);
                var weedHitted = Physics.Raycast(ray, out var hit, float.PositiveInfinity, weedMask);

                if (weedHitted && hit.collider.TryGetComponent<Weed>(out var weed))
                {
                    pulledWeed = weed;
                }
            }

            if (pulledWeed != null && Input.GetKeyUp(KeyCode.Mouse0))
            {
                var plane = new Plane(Camera.main.transform.forward, pulledWeed.transform.position);
                var input = Input.mousePosition;
                var camRay = Camera.main.ScreenPointToRay(input);
                if (plane.Raycast(camRay, out var planeDistance))
                {
                    var vector = camRay.GetPoint(planeDistance) - pulledWeed.transform.position;
                    Debug.Log("Weed pulled with force: " + vector.y);
                    if (vector.y >= pullThreshold)
                    {
                        pulledWeed.PullOutAndDestroy();
                        state.PullOutWeed();
                    }
                }

                pulledWeed = null;
            }
        }
    }

    public override void Clear()
    {
        pulledWeed = null;
        pullingOutStarted = false;
        PlantRotationManager.Instance.SetRotationEnabled(false);
        CameraManager.Instanse.ReturnToOriginalPosition();
    }
}
