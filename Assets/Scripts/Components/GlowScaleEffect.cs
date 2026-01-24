using UnityEngine;

public class GlowScaleEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private float effectDuration = 0.5f;

    private Vector3 originalScale;
    private Material originalMaterial;
    private Renderer objectRenderer;

    private float currentTime;
    private bool isEffectRunning;

    private const float MIN_SCALE = 0.01f;

    void Awake()
    {
        InitializeComponent();
    }

    void Update()
    {
        if (isEffectRunning)
        {
            UpdateEffect();
        }
    }

    void OnDestroy()
    {
        ResetToOriginalState();
    }

    public void TriggerEffect()
    {
        if (!isEffectRunning)
        {
            currentTime = 0f;
            isEffectRunning = true;
            enabled = true;
        }
    }

    private void InitializeComponent()
    {
        originalScale = transform.localScale;

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("GlowScaleEffect: No Renderer component found on this object!", this);
            return;
        }


        SaveOriginalEmissionState();

        isEffectRunning = false;
    }

    private void SaveOriginalEmissionState()
    {
        originalMaterial = objectRenderer.material;
        var emissionMaterial = new Material(originalMaterial);
        emissionMaterial.EnableKeyword("_EMISSION");
        objectRenderer.material = emissionMaterial;
    }

    private void UpdateEffect()
    {
        currentTime += Time.deltaTime;
        float progress = Mathf.Clamp01(currentTime / effectDuration);

        UpdateScale(progress);
        UpdateGlow(progress);

        if (currentTime >= effectDuration)
        {
            CompleteEffect();
        }
    }

    private void UpdateScale(float progress)
    {
        var scaleFactor = GetScaleIntensity(progress);
        transform.localScale = originalScale * scaleFactor;
    }

    private float GetScaleIntensity(float progress)
    {
        progress = Mathf.SmoothStep(0.0f, 1.0f, progress);
        if (progress < 0.9f)
            return Mathf.Lerp(MIN_SCALE, 1.2f, progress / 0.9f) ;
        else
            return Mathf.Lerp(1.2f, 1, (progress - 0.9f) / 0.1f);
    }

    private void UpdateGlow(float progress)
    {
        if (objectRenderer == null) return;

        float glowIntensity = GetGlowIntensity(progress);

        objectRenderer.material.SetColor("_EmissionColor", Color.white * glowIntensity);

        objectRenderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
    }

    private float GetGlowIntensity(float progress)
    {
        if (progress < 0.3f)
            return Mathf.Lerp(0f, 1f, progress / 0.3f) * 4;
        else
            return Mathf.Lerp(1f, 0f, (progress - 0.3f) / 0.7f) * 4;
    }

    private void CompleteEffect()
    {
        isEffectRunning = false;
        ResetToOriginalState();
        enabled = false;
    }

    private void ResetToOriginalState()
    {
        transform.localScale = originalScale;

        if (objectRenderer != null)
        {
            objectRenderer.material = originalMaterial;
        }
    }

    [ContextMenu("Test Effect")]
    private void TestEffect()
    {
        TriggerEffect();
    }
}