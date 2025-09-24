using System;
using Unity.VisualScripting;
using UnityEngine;

public class WateringState<TEvent> : State<TEvent>
    where TEvent : Enum
{
    private WateringScriptableStateInfo<TEvent> wateringStateInfo => (WateringScriptableStateInfo<TEvent>)StateInfo;
    public float wateringProgress;

    private ParticleSystem createdDustParticles;
    private ParticleHitTracker hitTracker;
    private MeshRenderer dirtMesh;
    private Material originalMaterial;
    private int waterLayer;

    private float currentWateringParticlesAmount;

    public WateringState(WateringScriptableStateInfo<TEvent> stateInfo) : base(stateInfo) { }

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
        dirtMesh.material = originalMaterial;
        dirtMesh.material.SetColor("_BaseColor", new Color(0.25f, 0.25f, 0.25f));
    }

    private void OnParticleHit(GameObject other)
    {
        if (other.layer == waterLayer && wateringProgress <= 1f)
        {
            currentWateringParticlesAmount++;
            wateringProgress = currentWateringParticlesAmount / wateringStateInfo.targetWateringParticlesAmount;
            var color = Color.Lerp(wateringStateInfo.dryColor, new Color(0.25f, 0.25f, 0.25f), wateringProgress);
            dirtMesh.material.SetColor("_BaseColor", color);
            if (wateringProgress >= 1f)
            {
                hitTracker.StopTracking();
            }
        }
    }
}