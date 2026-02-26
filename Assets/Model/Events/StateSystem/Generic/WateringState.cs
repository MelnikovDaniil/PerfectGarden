using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WateringState<TEvent> : State<TEvent>
    where TEvent : Enum
{
    private WateringScriptableStateInfo<TEvent> wateringStateInfo => (WateringScriptableStateInfo<TEvent>)StateInfo;
    public float wateringProgress;

    private ParticleSystem completeParticles;
    private ParticleSystem dustParticles;
    private ParticleSystem splashParticles;
    private ParticleHitTracker hitTracker;
    private MeshRenderer dirtMesh;
    private Material originalMaterial;
    private int waterLayer;
    private SMSound wateringSound;

    private Coroutine soundCoroutine;
    private float soundPlayTimeLeft;
    private float currentWateringParticlesAmount;

    public WateringState(WateringScriptableStateInfo<TEvent> stateInfo) : base(stateInfo) { }

    public override void Apply(PotWithPlant potWhithPlant)
    {
        waterLayer  = LayerMask.NameToLayer("WaterParticles");
        currentWateringParticlesAmount = 0;
        wateringProgress = 0;
        completeParticles = GameObject.Instantiate(wateringStateInfo.CompleteParticlesPrefab, potWhithPlant.dirtCollider.transform);
        dustParticles = GameObject.Instantiate(wateringStateInfo.DustParticlesPrefab, potWhithPlant.dirtCollider.transform);
        splashParticles = GameObject.Instantiate(wateringStateInfo.splashParticlesPrefab, potWhithPlant.dirtCollider.transform);
        dirtMesh = potWhithPlant.dirtCollider.GetComponent<MeshRenderer>();
        originalMaterial = dirtMesh.material;

        var dryMaterial = new Material(originalMaterial);
        dirtMesh.material = dryMaterial;
        dryMaterial.SetColor("_BaseColor", wateringStateInfo.dryColor);

        hitTracker = dirtMesh.AddComponent<ParticleHitTracker>();
        hitTracker.OnHit += OnParticleHit;
        hitTracker.StartTracking();
    }

    public void EnableSounds()
    {
        wateringSound = SoundManager.PlaySound(wateringStateInfo.wateringClip);
        wateringSound.SetLooped(true);
        wateringSound.Source.volume = 0;
    }

    public void DisableSounds()
    {
        wateringSound.SetLooped(false);
        wateringSound.Stop();
    }

    public override void Complete(PotWithPlant plant)
    {
        GameObject.Destroy(dustParticles.gameObject);
        GameObject.Destroy(splashParticles.gameObject);
        GameObject.Destroy(hitTracker);
        dirtMesh.material = originalMaterial;
        dirtMesh.material.SetColor("_BaseColor", new Color(0.25f, 0.25f, 0.25f));
    }

    private void OnParticleHit(GameObject other)
    {
        if (other.layer == waterLayer && wateringProgress <= 1f)
        {
            if (soundCoroutine == null)
            {
                soundCoroutine = hitTracker.StartCoroutine(PlaySoundRoutine());
            }
            soundPlayTimeLeft = 0.3f;
            var collisionEvents = new List<ParticleCollisionEvent>();
            var particleSystem = other.GetComponent<ParticleSystem>();
            var numCollisionEvents = particleSystem.GetCollisionEvents(dirtMesh.gameObject, collisionEvents);
            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 collisionPoint = collisionEvents[i].intersection;
                splashParticles.transform.position = collisionPoint;
                splashParticles.Emit(3);
            }

            currentWateringParticlesAmount++;
            wateringProgress = currentWateringParticlesAmount / wateringStateInfo.targetWateringParticlesAmount;
            var color = Color.Lerp(wateringStateInfo.dryColor, new Color(0.25f, 0.25f, 0.25f), wateringProgress);
            dirtMesh.material.SetColor("_BaseColor", color);
            if (wateringProgress >= 1f)
            {
                completeParticles.Play();
                SoundManager.PlaySound(wateringStateInfo.wateringCompleteClip);
                SoundManager.PlaySound(wateringStateInfo.wateringCompleteKalimbaClip);
                DisableSounds();
                hitTracker.StopTracking();
            }
        }
    }

    private IEnumerator PlaySoundRoutine()
    {
        while (wateringSound.Source != null && wateringProgress < 1)
        {
            if (soundPlayTimeLeft > 0)
            {
                wateringSound.Source.volume = 1;
                soundPlayTimeLeft -= Time.deltaTime;
            }
            else
            {
                wateringSound.Source.volume = 0;
            }
            yield return new WaitForEndOfFrame();
        }
        if (wateringSound.Source != null)
        {
            wateringSound.Source.volume = 0;
        }
        yield return null;
    }
}