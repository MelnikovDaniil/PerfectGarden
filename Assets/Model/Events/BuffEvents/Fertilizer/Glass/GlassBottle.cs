using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GlassBottle : MonoBehaviour
{
    public WaterSurface waterSurfacePrefab;
    public Vector3 waterSurfacePosition = new Vector3(0, -0.13f, 0);
    public Transform tipSurface;
    public Transform tip;

    [Header("Settings")]
    public float maxFillLevel = 1.0f;
    public Vector2 targetLevelRange = new Vector2(0.3f, 0.8f);
    public float tolerance = 0.1f;
    public int requiredShakeNumber = 5;

    private WaterSurface currentWaterSurface;
    private Animator _animator;
    private Shakeable _shaker;
    private DragAndDrop _dragAndDrop;
    private float targetLevel;
    private float currentLevel = 0.0f;
    private float shakingProgress;

    private List<WaterSurface> saves = new List<WaterSurface>();

    private void Awake()
    {
        _shaker = GetComponent<Shakeable>();
        _animator = GetComponent<Animator>();
        _dragAndDrop = GetComponent<DragAndDrop>();
    }

    private void Start()
    {
        Clear();
        _animator.Play("Glass_Idle", 0, 0);
        _shaker.SetShakeEnabled(false);
        _shaker.OnShake += ColorMix;
        _dragAndDrop.SetEnabled(false);
    }

    public void GenerateTargetLevel(bool lastLevel = false)
    {
        tip.parent = tipSurface;
        if (lastLevel)
        {
            targetLevel = 1f;
        }
        else
        {
            targetLevel = Random.Range(targetLevelRange.x, targetLevelRange.y);
        }
        tipSurface.localScale = new Vector3(1, targetLevel, 1);
        tip.parent = transform;
        tip.localScale = Vector3.one * tip.localScale.x;
        tip.gameObject.SetActive(true);

        var lastSurface = saves.LastOrDefault();
        currentWaterSurface = Instantiate(waterSurfacePrefab, transform);
        currentWaterSurface.transform.localScale = new Vector3(1, 0, 1);
        currentWaterSurface.level = targetLevel;

        if (lastSurface != null)
        {

            currentWaterSurface.transform.position = lastSurface.topPoint.position;
        }
        else
        {
            currentWaterSurface.transform.localPosition = waterSurfacePosition;
        }
    }

    public async Task CloseCapAsync()
    {
        await AnimatorHelper.PlayAnimationForTheEndAsync(_animator, "Glass_Close");
        tip.gameObject.SetActive(false);
    }

    public async Task StartShakingAsync(CancellationToken token = default)
    {
        _shaker.SetShakeEnabled(true);
        _dragAndDrop.SetEnabled(true);

        shakingProgress = 0;
        while (shakingProgress < 1f)
        {
            if (token.IsCancellationRequested)
            {
                throw new TaskCanceledException("Shaking interapted");
            }
            await Task.Yield();
        }

        _shaker.SetShakeEnabled(false);
        _dragAndDrop.SetEnabled(false);
    }

    public void UpdateWaterLevel(float pourAmount)
    {
        currentLevel = Mathf.Min(currentLevel + pourAmount, maxFillLevel);

        var surfaceStartLevel = saves.LastOrDefault()?.level ?? 0f;
        var newScale = currentWaterSurface.transform.localScale;
        newScale.y = currentLevel - surfaceStartLevel;
        currentWaterSurface.transform.localScale = newScale;
    }

    public GlassBottleLevel CheckWaterLevel()
    {
        if (Mathf.Abs(currentLevel - targetLevel) < tolerance)
        {
            return GlassBottleLevel.Great;
        }
        else if (Mathf.Abs(currentLevel - targetLevel) < 2 * tolerance)
        {
            return GlassBottleLevel.Good;
        }
        else
        {
            return GlassBottleLevel.TryAgain;
        }
    }

    public void ResetLevel()
    {
        currentLevel = saves.LastOrDefault()?.level ?? 0f;
        currentWaterSurface.transform.localScale = new Vector3(1, 0, 1);
    }

    public void SaveLevel()
    {
        saves.Add(currentWaterSurface);
    }

    public void Clear()
    {
        // get rid of water, highlight, etc.
        foreach (var surface in saves)
        {
            Destroy(surface.gameObject);
        }
        saves.Clear();
        currentLevel = 0.0f;
    }

    public void SetColor(Color randomColor)
    {
        currentWaterSurface.color = randomColor;
        currentWaterSurface.waterRenderer.material = new Material(currentWaterSurface.waterRenderer.material)
        {
            color = randomColor
        };
    }

    private void ColorMix()
    {
        // implement
        shakingProgress += 1f / requiredShakeNumber;
        var targetColor = MixColors(saves.Select(waterSurface => waterSurface.color));
        foreach (var surface in saves)
        {
            surface.waterRenderer.material.color = Color.Lerp(surface.color, targetColor, shakingProgress);
        }
    }

    public static Color MixColors(IEnumerable<Color> colors)
    {
        if (colors == null || colors.Count() == 0)
            return Color.black; // Return black if the array is empty or null

        var mixedColor = new Color(0, 0, 0, 0);

        foreach (var color in colors)
        {
            mixedColor += color;
        }

        mixedColor /= colors.Count();

        return mixedColor;
    }
}
