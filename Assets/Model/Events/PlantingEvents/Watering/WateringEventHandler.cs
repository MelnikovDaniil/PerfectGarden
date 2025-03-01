using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WateringEventHandler : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.Watering;

    public Animator waterignAnimator;
    public WateringCan wateringCan;
    public Transform potTransformParent;

    private bool plantWatered;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        plantWatered = false;
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;
        Context.PotWithPlant.transform.localPosition = Vector3.zero;
        Context.PotWithPlant.transform.localScale = Vector3.one;

        wateringCan.gameObject.SetActive(true);
        wateringCan.StopWatering();
        wateringCan.transform.localPosition = Vector3.zero;

        waterignAnimator.gameObject.SetActive(true);

        await PlayAnimationForTheEndAsync(waterignAnimator, "Appearance");
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        wateringCan.StartWaterging();
        Context.PotWithPlant.potWatering.StartWatering();
        Context.PotWithPlant.potWatering.OnPlantWatered = () => { plantWatered = true; };

        while (!plantWatered)
        {
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
