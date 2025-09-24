using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WateringEventHandler : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.Watering;

    public Animator waterignAnimator;
    public WateringCan wateringCan;
    public Transform potTransformParent;

    private WateringState<PlantingEvent> state;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        state = Context.PotWithPlant.GetState<WateringState<PlantingEvent>>();
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;
        Context.PotWithPlant.transform.localPosition = Vector3.zero;
        Context.PotWithPlant.transform.localScale = Vector3.one;

        wateringCan.gameObject.SetActive(true);
        wateringCan.StopWatering();
        wateringCan.transform.localPosition = Vector3.zero;

        waterignAnimator.gameObject.SetActive(true);

        await AnimatorHelper.PlayAnimationForTheEndAsync(waterignAnimator, "Appearance");
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
    }

    public override void Clear()
    {
        Context.PotWithPlant.gameObject.SetActive(false);
        wateringCan.StopWatering();
        wateringCan.gameObject.SetActive(false);
        waterignAnimator.gameObject.SetActive(false);
    }
}
