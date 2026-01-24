using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WateringEventHandler : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.Watering;

    public WateringCan wateringCan;
    public Transform potTransformParent;

    private WateringState<PlantingEvent> state;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        state = Context.PotWithPlant.GetState<WateringState<PlantingEvent>>();
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

    public override void Clear()
    {
        Context.PotWithPlant.gameObject.SetActive(false);
        wateringCan.StopWatering();
        wateringCan.gameObject.SetActive(false);
    }
}
