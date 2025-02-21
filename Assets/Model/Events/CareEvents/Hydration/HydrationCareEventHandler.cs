using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HydrationCareEventHandler : CareEventHandler
{
    public int spraysAmount = 4;
    public Vector2 sprayDistanceRange = new Vector2(1f, 2f);
    public Animator sprayAnimatorPrefab;

    private Animator createdSprayAnimator;
    private int currentSpraysAmount;

    private bool isSpraying = false;

    private LayerMask sprayLayerMask;

    public override CareEvent EventName => CareEvent.Hydration;

    private void Awake()
    {
        sprayLayerMask = LayerMask.GetMask("Spray");
        createdSprayAnimator = Instantiate(sprayAnimatorPrefab);
        createdSprayAnimator.gameObject.SetActive(false);
        createdSprayAnimator.gameObject.layer = LayerMask.NameToLayer("Spray");
        GenerateSprayPosition();
    }

    protected override Task PrepareHandlingAsync(CancellationToken token = default)
    {
        createdSprayAnimator.transform.parent = Context.CarePlace;
        createdSprayAnimator.transform.localScale = Vector3.one;

        isSpraying = false;
        currentSpraysAmount = spraysAmount;
        MoveSpray(createdSprayAnimator.transform, GenerateSprayPosition());
        createdSprayAnimator.gameObject.SetActive(true);

        
        return base.PrepareHandlingAsync(token);
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        isSpraying = true;
        var previousSpraysAmount = currentSpraysAmount;
        while (currentSpraysAmount > 0)
        {
            if (currentSpraysAmount < previousSpraysAmount)
            {
                previousSpraysAmount = currentSpraysAmount;
                await MakeSprayAsync();
            }

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
        if (isSpraying)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var input = Input.mousePosition;
                var mousePosition = Camera.main.ScreenToWorldPoint(input);
                var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(camRay, float.PositiveInfinity, sprayLayerMask))
                {
                    currentSpraysAmount--;
                }
            }
            /// Add logic to find spray, decrease counter and change sprayPosition eachTime
        }
    }

    public override void Clear()
    {
        isSpraying = false;
        currentSpraysAmount = spraysAmount;
        createdSprayAnimator.gameObject.SetActive(false);
    }

    private async Task MakeSprayAsync()
    {
        await PlayAnimationForTheEndAsync(createdSprayAnimator, "Spray");
        MoveSpray(createdSprayAnimator.transform, GenerateSprayPosition());
    }

    private void MoveSpray(Transform sprayTransform, Vector3 position)
    {
        sprayTransform.position = position + Context.PotWithPlant.plantRenderer.transform.position;
        sprayTransform.localScale = new Vector2(
            sprayTransform.localScale.x,
            -Mathf.Sign(sprayTransform.position.x) * Mathf.Abs(sprayTransform.localScale.y));

        var oppositeDirection = -position;
        var angle = Mathf.Atan2(oppositeDirection.y, oppositeDirection.x) * Mathf.Rad2Deg;
        sprayTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector2 GenerateSprayPosition()
    {
        var sprayPositionNormalized = new Vector2(
            Random.Range(-1f, 1f),
            Random.value).normalized;
        var index = Random.Range(sprayDistanceRange.x, sprayDistanceRange.y);

        return sprayPositionNormalized * index;
    }
}
