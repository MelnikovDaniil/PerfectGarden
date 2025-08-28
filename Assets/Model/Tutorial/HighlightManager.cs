using UnityEngine;
using UnityEngine.UI;

public class HighlightManager : MonoBehaviour
{
    public Image darkBackground;
    public Image border;
    public float padding = 10f;

    [Header("Mask Settings")]
    public Color baseColor = new Color(0, 0, 0, 0.5f);
    public float minMaskRadius = 0.25f;

    private float aspectRatio;
    private Material cutoutMaterial;

    void Awake()
    {
        if (darkBackground == null)
        {
            Debug.LogError("Dark Background is not assigned!");
            return;
        }

        aspectRatio = Camera.main.aspect;
        cutoutMaterial = new Material(darkBackground.material);
        darkBackground.material = cutoutMaterial;

        darkBackground.gameObject.SetActive(false);
        UpdateShaderProperties();
    }

    public void HighlightObject(GameObject target)
    {
        StopHighlight();

        darkBackground.gameObject.SetActive(true);

        Bounds bounds;
        if (target.TryGetComponent(out RectTransform rectTransform))
        {
            bounds = GetUIBounds(rectTransform);
        }
        else
        {
            bounds = Get3DBounds(target);
        }

        var min = Camera.main.WorldToScreenPoint(bounds.min);
        var max = Camera.main.WorldToScreenPoint(bounds.max);

        if (target.GetComponent<RectTransform>() != null)
        {
            min = bounds.min;
            max = bounds.max;
        }

        var maskCenter = new Vector2(
            ((min.x + max.x) / 2f) / Screen.width,
            ((min.y + max.y) / 2f) / Screen.height
        );

        var width = Mathf.Abs(max.x - min.x);
        var height = Mathf.Abs(max.y - min.y);
        var radius = Mathf.Sqrt(width * width + height * height) / (2 * Screen.height);
        radius += padding / Screen.height;
        radius = Mathf.Max(radius, minMaskRadius);

        border.rectTransform.anchorMin = maskCenter;
        border.rectTransform.anchorMax = maskCenter;
        border.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        var diameter = radius * 2 * Screen.height;
        border.rectTransform.sizeDelta = new Vector2(diameter, diameter);

        cutoutMaterial.SetVector("_MaskCenter", new Vector4(maskCenter.x, maskCenter.y, 0, 0));
        cutoutMaterial.SetFloat("_MaskRadius", radius);
        UpdateShaderProperties();
    }

    public void StopHighlight()
    {
        if (darkBackground != null)
        {
            darkBackground.gameObject.SetActive(false);
        }
    }

    private void UpdateShaderProperties()
    {
        if (cutoutMaterial != null)
        {
            cutoutMaterial.SetColor("_BaseColor", baseColor);
            cutoutMaterial.SetFloat("_AspectRatio", aspectRatio);
        }
    }

    private Bounds Get3DBounds(GameObject target)
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(target.transform.position, Vector3.zero);

        var bounds = renderers[0].bounds;
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    private Bounds GetUIBounds(RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        var min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        var max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        return new Bounds((min + max) / 2, max - min);
    }
}