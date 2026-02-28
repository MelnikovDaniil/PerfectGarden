using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HydrationCareEventHandler : CareEventHandler
{
    public int spraysAmount = 4;
    public Vector2 sprayDistanceRange = new Vector2(1f, 2f);
    public Vector2 sprayOffset = new Vector2(0, 1);
    public HydrationSpray sprayPrefab;
    public AudioClip completeClip;

    private HydrationSpray sprayInstance;
    private int currentSpraysAmount;

    private bool isSpraying = false;

    private LayerMask sprayLayerMask;

    public override CareEvent EventName => CareEvent.Hydration;

    private void Awake()
    {
        sprayLayerMask = LayerMask.GetMask("Spray");
        sprayInstance = Instantiate(sprayPrefab);
        sprayInstance.gameObject.SetActive(false);
        sprayInstance.gameObject.layer = LayerMask.NameToLayer("Spray");
    }

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        sprayInstance.transform.parent = Context.CarePlace;
        sprayInstance.transform.localScale = Vector3.one;

        isSpraying = false;
        currentSpraysAmount = spraysAmount;
        sprayInstance.MoveSprayRelatively(Context.PotWithPlant.plantRenderer.transform, sprayInstance.GenerateSprayPosition(sprayDistanceRange), sprayOffset);
        sprayInstance.gameObject.SetActive(true);
        var targetPosition = sprayInstance.transform.position;
        sprayInstance.transform.localPosition += new Vector3(5 * Mathf.Sign(sprayInstance.transform.localPosition.x), 0, 0);
        await MovementHelper.MoveObjectToTargetAsync(sprayInstance.transform, targetPosition, 1, true);
        await MovementHelper.MoveObjectToBasePositionAsync(Context.PotWithPlant.transform, 1, true);
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        isSpraying = true;
        var previousSpraysAmount = currentSpraysAmount;
        while (currentSpraysAmount > 0 || currentSpraysAmount != previousSpraysAmount)
        {
            await Task.Yield();
            await TutorialManager.Instance.SetTap(sprayInstance.hintPlace.gameObject, false, token);
            if (currentSpraysAmount < previousSpraysAmount)
            {
                previousSpraysAmount = currentSpraysAmount;
                await sprayInstance.MakeSprayAsync(Context.PotWithPlant.plantRenderer.transform, sprayDistanceRange, sprayOffset);
            }

            if (token.IsCancellationRequested)
            {
                await InterruptAsync();
                return;
            }
        }

        SoundManager.PlaySound(completeClip);
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
        sprayInstance.gameObject.SetActive(false);
    }
}
