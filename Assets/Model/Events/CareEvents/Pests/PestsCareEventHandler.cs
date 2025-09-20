using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PestsCareEventHandler : CareEventHandler
{
    public override CareEvent EventName => CareEvent.Pests;

    public WormPincet pincetPrefab;

    [Header("Settings")]
    public float cameraZoom = 6;
    public Vector3 cameraOffset;

    private LayerMask pestsMask;
    private bool pullingOutStarted = false;

    private PestsState state;
    private WormPincet pincetInstance;

    private void Awake()
    {
        pestsMask = LayerMask.GetMask("Pests");
    }

    private void Start()
    {
        pincetInstance = Instantiate(pincetPrefab, transform);
        pincetInstance.gameObject.SetActive(false);
    }

    protected override Task PrepareHandlingAsync(CancellationToken token = default)
    {
        state = Context.PotWithPlant.GetState<PestsState>();
        pincetInstance.gameObject.SetActive(true);
        pincetInstance.transform.parent = Context.PotWithPlant.transform;
        pincetInstance.transform.localPosition = Vector2.up * 0.28f;
        pincetInstance.transform.localScale = Vector2.one;
        CameraManager.Instanse.LookAtPoint(state.GetGroupPosition(), cameraZoom, cameraOffset);
        return Task.CompletedTask;
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        foreach (var worm in state.worms)
        {
            _ = TutorialManager.Instance.SetTap(worm.gameObject, false, token);
        }
        pullingOutStarted = true;
        while (state.wormNumberLeft > 0)
        {
            if (token.IsCancellationRequested)
            {
                await InterruptAsync();
                return;
            }
            await Task.Yield();
        }
    }

    private async void Update()
    {
        if (pullingOutStarted)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var input = Input.mousePosition;
                var ray = Camera.main.ScreenPointToRay(input);
                var weedHitted = Physics.Raycast(ray, out var hit, float.PositiveInfinity, pestsMask);

                if (weedHitted && hit.collider.TryGetComponent<Worm>(out var worm))
                {
                    var isSuccess = worm.TryPullOut();
                    StartCoroutine(PullingOutRoutine(worm.gameObject, isSuccess));
                }
            }
        }
    }

    public IEnumerator PullingOutRoutine(GameObject obj, bool isSuccess)
    {
        yield return pincetInstance.GrabRoutine(obj, isSuccess);
        if (isSuccess)
        {
            state.PullOutWorm();
        }
    }

    public override void Clear()
    {
        pincetInstance.gameObject.SetActive(false);
        pincetInstance.transform.parent = transform;
        pullingOutStarted = false;
        CameraManager.Instanse.ReturnToOriginalPosition();
    }
}
