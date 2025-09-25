using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WateringCareEventHandler : CareEventHandler
{
    public override CareEvent EventName => CareEvent.Watering;

    public Animator waterignAnimator;
    public WateringCan wateringCan;
    public Transform potTransformParent;

    private WateringState<CareEvent> state;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        // Disable care rotation
        state = Context.PotWithPlant.GetState<WateringState<CareEvent>>();
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

    protected override Task InterruptHandlingAsync()
    {
        // enable care rotation
        return base.InterruptHandlingAsync();
    }

    public override void Clear()
    {
        // enable care rotation
        wateringCan.StopWatering();
        wateringCan.gameObject.SetActive(false);
        waterignAnimator.gameObject.SetActive(false);
    }
}
