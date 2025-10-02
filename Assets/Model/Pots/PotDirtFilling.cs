using System;
using UnityEngine;

public class PotDirtFilling : MonoBehaviour
{
    public event Action OnPotFill;

    [Space]
    public Transform dirt;
    public float dirtMinY = -0.3f;
    public float dirtMaxY = 0.5f;
    public float targetFillParticlesAmount = 100;

    private Vector3 dirtScale = Vector3.one * 0.7f;
    private float fillingProgress;
    private float currentFillParticlesAmount;
    private int dirtLayer;

    private void Awake()
    {
        dirtLayer = LayerMask.NameToLayer("DirtParticles");
    }

    public void StartFilling()
    {
        currentFillParticlesAmount = 0;
        dirt.localPosition = new Vector3(0, dirtMinY, 0);
        dirt.localScale = dirtScale;
        dirt.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == dirtLayer && fillingProgress < 1f)
        {
            currentFillParticlesAmount++;
            fillingProgress = currentFillParticlesAmount / targetFillParticlesAmount;
            var newY = dirtMinY + (dirtMaxY - dirtMinY) * fillingProgress;
            dirt.localPosition = new Vector3(0, Mathf.Clamp(newY, dirtMinY, dirtMaxY), 0);
            dirt.localScale = Vector3.Lerp(dirtScale, Vector3.one, fillingProgress);
            if (fillingProgress >= 1)
            {
                OnPotFill?.Invoke();
            }
        }
    }
}
