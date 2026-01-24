using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WateringCareEventHandler : CareEventHandler
{
    public override CareEvent EventName => CareEvent.Watering;

    public WateringCan wateringCan;
    public Transform potTransformParent;

    private WateringState<CareEvent> state;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        // Disable care rotation
        state = Context.PotWithPlant.GetState<WateringState<CareEvent>>();
        state.EnableSounds();
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;

        wateringCan.gameObject.SetActive(true);
        wateringCan.StopWatering();
        wateringCan.transform.localPosition = new Vector3(0, 12f, 0);

        await MovementHelper.MoveObjectToBasePositionAsync(wateringCan.transform, 1, true);
        await MovementHelper.MoveObjectToBasePositionAsync(Context.PotWithPlant.transform, 1, true);
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        _ = TutorialManager.Instance.SetHoldAsync(wateringCan.gameObject, 1f, false, token);
        wateringCan.StartWaterging();

        while (state.wateringProgress < 1f)
        {
            if (token.IsCancellationRequested)
            {
                await InterruptAsync();
                return;
            }
            await Task.Yield();
        }
        await Task.Delay(1500);
        await MovementHelper.MoveObjectAwayAsync(wateringCan.transform, Vector3.up, 0.3f, true);
    }

    protected override async Task InterruptHandlingAsync()
    {
        state.DisableSounds();
        await MovementHelper.MoveObjectAwayAsync(wateringCan.transform, Vector3.up, 0.3f, true);
        await base.InterruptHandlingAsync();
    }

    public override void Clear()
    {
        // enable care rotation
        wateringCan.StopWatering();
        wateringCan.gameObject.SetActive(false);
    }
}
