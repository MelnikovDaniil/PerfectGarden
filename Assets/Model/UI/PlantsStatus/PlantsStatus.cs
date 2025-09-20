using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlantsStatusUI : MonoBehaviour
{
    public Canvas canvas;
    public Image iconPrefab;
    public Vector3 offset;
    public float iconSize = 30;

    public Sprite needsCareSprite;
    public Sprite deadSprite;

    private List<Image> iconsPool = new List<Image>();
    private List<PotWithPlant> plants = new List<PotWithPlant>();

    private void Awake()
    {
        GardernManager.OnGardenOpen += ShowIcons;
        GardernManager.OnGardenClose += HideIcons;
    }

    private void Start()
    {
        plants = GardernManager.Instance.growingPlants;
        ShowIcons();
    }

    private void ShowIcons()
    {
        canvas.gameObject.SetActive(true);
        foreach (var plant in plants)
        {
            UpdateIconSprite(plant);
        }
    }

    private void HideIcons()
    {
        canvas.gameObject.SetActive(false);
    }

    private void UpdateIconSprite(PotWithPlant plant)
    {
        var icon = iconsPool.FirstOrDefault(x => !x.gameObject.activeSelf);
        if (icon == null)
        {
            icon = Instantiate(iconPrefab, canvas.gameObject.transform);
            icon.rectTransform.sizeDelta = Vector2.one * iconSize;
            iconsPool.Add(icon);
        }

        var iconPosition = Camera.main.WorldToScreenPoint(plant.transform.position + offset);
        icon.transform.position = iconPosition;
        icon.gameObject.SetActive(true);

        if (plant.IsDied)
        {
            icon.sprite = deadSprite;
        }
        else if (plant.waitingCareEvents.Any())
        {
            icon.sprite = needsCareSprite;
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }
}
