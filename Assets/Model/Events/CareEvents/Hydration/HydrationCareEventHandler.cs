using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class HydrationCareEventHandler : CareEventHandler
{
    public int spraysAmount = 4;
    public Vector2 sprayDistanceRange = new Vector2(1f, 2f);
    public HydrationSpray sprayPrefab;

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

    protected override Task PrepareHandlingAsync(CancellationToken token = default)
    {
        sprayInstance.transform.parent = Context.CarePlace;
        sprayInstance.transform.localScale = Vector3.one;

        isSpraying = false;
        currentSpraysAmount = spraysAmount;
        sprayInstance.MoveSprayRelatively(Context.PotWithPlant.plantRenderer.transform, sprayInstance.GenerateSprayPosition(sprayDistanceRange));
        sprayInstance.gameObject.SetActive(true);


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
                await sprayInstance.MakeSprayAsync(Context.PotWithPlant.plantRenderer.transform, sprayDistanceRange);
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
        sprayInstance.gameObject.SetActive(false);
    }
}
