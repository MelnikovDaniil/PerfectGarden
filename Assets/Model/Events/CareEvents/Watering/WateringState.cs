using Unity.VisualScripting;
using UnityEngine;

public class WateringState : CareState
{
    private WateringStateScriptableStateInfo wateringStateInfo => (WateringStateScriptableStateInfo)StateInfo;
    public float wateringProgress;

    private ParticleSystem createdDustParticles;
    private ParticleHitTracker hitTracker;
    private MeshRenderer dirtMesh;
    private Material originalMaterial;
    private int waterLayer;

    private float currentWateringParticlesAmount;

    public WateringState(WateringStateScriptableStateInfo stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant potWhithPlant)
    {
        waterLayer  = LayerMask.NameToLayer("WaterParticles");
        currentWateringParticlesAmount = 0;
        wateringProgress = 0;
        createdDustParticles = GameObject.Instantiate(wateringStateInfo.DustParticlesPrefab, potWhithPlant.dirtCollider.transform);
        dirtMesh = potWhithPlant.dirtCollider.GetComponent<MeshRenderer>();
        originalMaterial = dirtMesh.material;

        var dryMaterial = new Material(originalMaterial);
        dirtMesh.material = dryMaterial;
        dryMaterial.SetColor("_BaseColor", wateringStateInfo.dryColor);

        hitTracker = dirtMesh.AddComponent<ParticleHitTracker>();
        hitTracker.OnHit += OnParticleHit;
        hitTracker.StartTracking();
    }

    public override void Complete(PotWithPlant plant)
    {
        GameObject.Destroy(createdDustParticles.gameObject);
        GameObject.Destroy(hitTracker);
        plant.plantRenderer.material = originalMaterial;
    }

    private void OnParticleHit(GameObject other)
    {
        if (other.layer == waterLayer && wateringProgress <= 1f)
        {
            currentWateringParticlesAmount++;
            wateringProgress = currentWateringParticlesAmount / wateringStateInfo.targetWateringParticlesAmount;
            var color = Color.Lerp(wateringStateInfo.dryColor, Color.white, wateringProgress);
            dirtMesh.material.SetColor("_BaseColor", color);
            if (wateringProgress >= 1f)
            {
                hitTracker.StopTracking();
            }
        }
    }
}