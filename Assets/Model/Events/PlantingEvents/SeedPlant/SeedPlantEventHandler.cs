using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SeedPlantEventHandler : PlantEventHandler
{
    public AudioClip seedPlantClip;
    public AudioClip magicSpinningClip;

    public ParticleSystem seedCollisionParticlesPrefab;
    public ParticleSystem chargingParticlesPrefab;
    public ParticleSystem exploadParticlesPrefab;
    public ParticleSystem loopedHighlightParticlesPrefab;

    public Animator seedPlantAnimator;
    public Transform potTransformParent;
    public SeedPackage seedPackage;
    public Vector3 cameraOffset = new Vector3(1, 1.5f , 0);

    [SerializeField] private FinishPlanting finishPlantingCanvas;

    private ParticleSystem seedCollisionParticles;
    private ParticleSystem chargingParticles;
    private ParticleSystem exploadParticles;
    private ParticleSystem loopedHighlightParticles;

    private bool seedPlant;
    private bool plantingFinished;
    private GlowScaleEffect glowScaleEffect;
    private RotationEffect rotationEffect;
    private MovementEffect movementEffect;

    public override PlantingEvent EventName => PlantingEvent.SeedPlant;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        if (chargingParticles == null)
        {
            seedCollisionParticles = Instantiate(seedCollisionParticlesPrefab);
            chargingParticles = Instantiate(chargingParticlesPrefab);
            exploadParticles = Instantiate(exploadParticlesPrefab);
            loopedHighlightParticles = Instantiate(loopedHighlightParticlesPrefab);
        }

        seedPlant = false;
        plantingFinished = false;
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;
        Context.PotWithPlant.transform.localPosition = Vector3.zero;


        seedPackage.gameObject.SetActive(true);
        seedPackage.interactable = false;
        seedPackage.SetUp(Context.plantInfo.packageSprite, Context.plantInfo.seedSprite);
        seedPackage.transform.localPosition = Vector3.zero;

        seedPlantAnimator.gameObject.SetActive(true);

        seedCollisionParticles.transform.position = Context.PotWithPlant.dirtCollider.transform.position;
        chargingParticles.transform.position = Context.PotWithPlant.dirtCollider.transform.position;
        chargingParticles.gameObject.SetActive(false);
        chargingParticles.Stop();
        exploadParticles.transform.position = Context.PotWithPlant.dirtCollider.transform.position;
        exploadParticles.gameObject.SetActive(false);
        exploadParticles.Stop();
        loopedHighlightParticles.transform.position = Context.PotWithPlant.dirtCollider.transform.position;
        loopedHighlightParticles.gameObject.SetActive(false);
        loopedHighlightParticles.Stop();


        finishPlantingCanvas.finishPlantingButton.onClick.RemoveAllListeners();
        finishPlantingCanvas.finishPlantingButton.onClick.AddListener(() =>
        {
            plantingFinished = true;
            finishPlantingCanvas.HideMenu();
        });

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

        seedCollisionParticles.Play();
        CameraManager.Instanse.LookAtPoint(Context.PotWithPlant.dirtCollider.transform.position, 6f, cameraOffset);
        await Task.Delay(1000);
        SoundManager.PlaySound(seedPlantClip);
        chargingParticles.gameObject.SetActive(true);
        chargingParticles.Play();
        CameraManager.Instanse.ShakeCamera(0.1f, 3, 100f, 0.1f, 0.5f);
        CameraManager.Instanse.LookAtPoint(Context.PotWithPlant.dirtCollider.transform.position, 5f, cameraOffset, 3f);
        await Task.Delay(3000);
        CameraManager.Instanse.ShakeCamera(0);
        CameraManager.Instanse.LookAtPoint(Context.PotWithPlant.dirtCollider.transform.position, 5.5f, cameraOffset, 0.5f);
        await Task.Delay(500);
        exploadParticles.gameObject.SetActive(true);
        exploadParticles.Play();
        CameraManager.Instanse.LookAtPoint(Context.PotWithPlant.dirtCollider.transform.position, 4.5f, cameraOffset, 0.5f);
        CameraManager.Instanse.ShakeCamera(0.7f, 0.5f, 100f, 0.7f, 0);
        Context.PotWithPlant.SetStage(0);
        glowScaleEffect = Context.PotWithPlant.plantPlace.GetChild(0).AddComponent<GlowScaleEffect>();
        glowScaleEffect.TriggerEffect();
        Context.PotWithPlant.UpdateStageChangeTime();
        await Task.Delay(500);
        SoundManager.PlaySound(magicSpinningClip);
        rotationEffect = Context.PotWithPlant.AddComponent<RotationEffect>();
        rotationEffect.Rotate(Vector3.up, 5 * 360, 2.5f);
        loopedHighlightParticles.gameObject.SetActive(true);
        loopedHighlightParticles.Play();
        await Task.Delay(3000);
        movementEffect = Context.PotWithPlant.AddComponent<MovementEffect>();
        movementEffect.MoveToPointAndBack(Context.PotWithPlant.transform.position + Vector3.up * 0.5f, 5);

        CameraManager.Instanse.ReturnToOriginalPosition();

        finishPlantingCanvas.ShowMenu();
        while (!plantingFinished)
        {
            await Task.Yield();
        }
    }

    public override void Clear()
    {
        Destroy(glowScaleEffect);
        Destroy(movementEffect);
        Destroy(rotationEffect);
        chargingParticles.gameObject.SetActive(false);
        exploadParticles.gameObject.SetActive(false);
        loopedHighlightParticles.gameObject.SetActive(false);
        chargingParticles.gameObject.SetActive(false);
        exploadParticles.gameObject.SetActive(false);
        loopedHighlightParticles.gameObject.SetActive(false);
        Context.PotWithPlant.gameObject.SetActive(false);
        seedPackage.gameObject.SetActive(true);
        seedPackage.interactable = false;
        seedPlantAnimator.gameObject.SetActive(false);
    }
}
