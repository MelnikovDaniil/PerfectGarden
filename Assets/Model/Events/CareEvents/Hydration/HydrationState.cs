using UnityEngine;

public class HydrationState : CareState
{
    private HydrationScriptableStateInfo hydrationStateInfo => (HydrationScriptableStateInfo)StateInfo;

    private ParticleSystem createdDustParticles;
    private ParticleSystem createdDustSplash;
    private Material savedMaterial;

    public HydrationState(HydrationScriptableStateInfo stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant plant)
    {

        createdDustParticles = GameObject.Instantiate(hydrationStateInfo.DustParticlesPrefab, plant.transform);
        createdDustParticles.transform.localPosition = hydrationStateInfo.dustOffset;

        createdDustSplash = GameObject.Instantiate(hydrationStateInfo.dustSplashPrefab, plant.transform);
        createdDustSplash.transform.localPosition = hydrationStateInfo.dustOffset;
        var splashMain = createdDustSplash.main;
        splashMain.stopAction = ParticleSystemStopAction.Destroy;

        savedMaterial = plant.plantRenderer.materials[0];
        var existingTexture = savedMaterial.GetTexture("_MainTex");

        var dustMaterial = new Material(hydrationStateInfo.DustMaterial);
        dustMaterial.SetTexture("_MainTex", existingTexture);
        plant.plantRenderer.material = dustMaterial;
    }

    // Think about processing state, where action is not completed yet but during care it changing
    // To do that, Plant should have ProcessState(CareEven eventName) method,
    // and also completion progress bar shold be added to CareSate class

    public override void Complete(PotWithPlant plant)
    {
        createdDustSplash.Play();
        SoundManager.PlaySound(hydrationStateInfo.dustSplashClip);
        GameObject.Destroy(createdDustParticles.gameObject);
        plant.plantRenderer.material = savedMaterial;
    }
}