using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SeedPlantEventHandler : PlantEventHandler
{
    public Animator seedPlantAnimator;
    public Transform potTransformParent;
    public SeedPackage seedPackage;

    private bool seedPlant;
    public override PlantingEvent EventName => PlantingEvent.SeedPlant;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        seedPlant = false;
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;
        Context.PotWithPlant.transform.localPosition = Vector3.zero;


        seedPackage.gameObject.SetActive(true);
        seedPackage.interactable = false;
        seedPackage.SetUp(Context.PackageSprite, Context.SeedSprite);
        seedPackage.transform.localPosition = Vector3.zero;

        seedPlantAnimator.gameObject.SetActive(true);

        await AnimatorHelper.PlayAnimationForTheEndAsync(seedPlantAnimator, "Appearance");
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        _ = TutorialManager.Instance.SetTap(seedPackage.gameObject, false, token);
        seedPackage.interactable = true;
        Context.PotWithPlant.OnSeedPlant += () =>
        {

            Context.PotWithPlant.plantInfo = Context.plantInfo;
            seedPlant = true;
            Debug.Log("Seed Planted");
        };

        while (!seedPlant)
        {
            await Task.Yield();
        }
    }

    public override void Clear()
    {
        Context.PotWithPlant.gameObject.SetActive(false);
        seedPackage.gameObject.SetActive(true);
        seedPackage.interactable = false;
        seedPlantAnimator.gameObject.SetActive(false);
    }
}
