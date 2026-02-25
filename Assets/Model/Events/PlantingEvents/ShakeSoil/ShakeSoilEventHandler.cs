using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ShakeSoilPlantEvent : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.SoilShake;

    public Animator potFillingAnimator;
    public ShakingSoil shakingSoil;
    public Transform potTransformParent;
    public AudioClip soilAppearanceClip;
    public AudioClip completeClip;
    public ParticleSystem completeParticlesPrefab;
    public List<AudioClip> levelFillClips;

    private bool potFilled;
    private ParticleSystem completeParticles;

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        potFilled = false;
        Context.PotWithPlant.gameObject.SetActive(true);
        Context.PotWithPlant.transform.parent = potTransformParent;
        Context.PotWithPlant.transform.localPosition = Vector3.zero;
        Context.PotWithPlant.transform.localScale = Vector3.one;

        shakingSoil.gameObject.SetActive(true);
        shakingSoil.transform.localPosition = Vector3.up;
        shakingSoil.transform.localRotation = Quaternion.identity;

        potFillingAnimator.gameObject.SetActive(true);
        completeParticles = Instantiate(completeParticlesPrefab);
        completeParticles.transform.parent = Context.PotWithPlant.dirtCollider.transform;
        completeParticles.transform.localPosition = Vector3.zero;
        completeParticles.transform.localScale = Vector3.one;
        var main = completeParticles.main;
        main.playOnAwake = false;
        main.stopAction = ParticleSystemStopAction.Destroy;

        SoundManager.PlaySound(soilAppearanceClip);
        await AnimatorHelper.PlayAnimationForTheEndAsync(potFillingAnimator, "Appearance");
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        Context.PotWithPlant.potDirtFilling.StartFilling();
        shakingSoil.StartShaking();
        _ = TutorialManager.Instance.SetShake(shakingSoil.gameObject, false, token);
        Context.PotWithPlant.potDirtFilling.OnPotFillLevel += () => SoundManager.PlaySound(levelFillClips.GetRandom());
        Context.PotWithPlant.potDirtFilling.OnPotFillComplete += () => { potFilled = true; };

        while (!potFilled)
        {
            await Task.Yield();
        }

        completeParticles.Play();
        SoundManager.PlaySound(completeClip);
        var main = completeParticles.main;
        await Task.Delay((int)(1000 * main.duration));
        await MovementHelper.MoveObjectAwayAsync(shakingSoil.transform, Vector3.up, 0.3f, true);
    }

    public override void Clear()
    {
        if (completeParticles != null)
        {
            Destroy(completeParticles.gameObject);
        }
        Context.PotWithPlant.gameObject.SetActive(false);
        shakingSoil.StopShaking();
        shakingSoil.gameObject.SetActive(false);
        potFillingAnimator.gameObject.SetActive(false);
    }
}
