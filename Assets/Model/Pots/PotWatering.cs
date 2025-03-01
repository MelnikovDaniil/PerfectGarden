using System;
using UnityEngine;

public class PotWatering : MonoBehaviour
{
    public Action OnPlantWatered;

    [Space]
    public float targetWateringParticlesAmount = 70;
    public float targetWateringBrightness = 0.5f;

    [Space]
    public MeshRenderer dirtRenderer;
    private float wateringProgress;
    private int waterLayer;

    private float currentWateringParticlesAmount;

    private void Awake()
    {
        waterLayer = LayerMask.NameToLayer("WaterParticles");
    }

    public void StartWatering()
    {
        currentWateringParticlesAmount = 0;
        wateringProgress = 0;
        dirtRenderer.sharedMaterial.color = Color.white;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == waterLayer && wateringProgress < 1f)
        {
            currentWateringParticlesAmount++;
            wateringProgress = currentWateringParticlesAmount / targetWateringParticlesAmount;
            dirtRenderer.sharedMaterial.color = Color.HSVToRGB(0, 0, 1f - (wateringProgress) * targetWateringBrightness);
            if (wateringProgress >= 1)
            {
                OnPlantWatered?.Invoke();
            }
        }

    }
}
