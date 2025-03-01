using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ShakeSoilPlantEvent : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.SoilShake;

    public Animator potFillingAnimator;
    public ShakingSoil shakingSoil;
    public Transform potTransformParent;

    private bool potFilled;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        potFilled = false;
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;
        Context.PotWithPlant.transform.localPosition = Vector3.zero;
        Context.PotWithPlant.transform.localScale = Vector3.one;

        shakingSoil.gameObject.SetActive(true);
        shakingSoil.StopShaking();
        shakingSoil.transform.localPosition = Vector3.zero;
        shakingSoil.transform.localRotation = Quaternion.identity;

        potFillingAnimator.gameObject.SetActive(true);

        await PlayAnimationForTheEndAsync(potFillingAnimator, "Appearance");
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        shakingSoil.StartShaking();
        Context.PotWithPlant.potDirtFilling.StartFilling();
        Context.PotWithPlant.potDirtFilling.OnPotFill += () => { potFilled = true; };

        while (!potFilled)
        {
            await Task.Yield();
        }
    }

    public override void Clear()
    {
        Context.PotWithPlant.gameObject.SetActive(false);
        shakingSoil.StopShaking();
        shakingSoil.gameObject.SetActive(false);
        potFillingAnimator.gameObject.SetActive(false);
    }
}
