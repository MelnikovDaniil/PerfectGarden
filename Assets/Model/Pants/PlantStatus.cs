using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlantStatus : MonoBehaviour
{
    public Sprite diedIcon;
    public Sprite needsCareIcon;
    public Image statusIcon;
    private PotWithPlant plant;

    void Awake()
    {
        plant = GetComponent<PotWithPlant>();
    }

    public void ShowPlantStatus()
    {
        statusIcon.canvas.worldCamera = CameraManger.Instanse.uiCamera;
        statusIcon.transform.forward = CameraManger.Instanse.uiCamera.transform.forward;
        statusIcon.gameObject.SetActive(true);
        if (plant.IsDied)
        {
            statusIcon.sprite = diedIcon;
        }
        else if (plant.waitingCareEvents.Any())
        {
            statusIcon.sprite = needsCareIcon;
        }
        else
        {
            statusIcon.gameObject.SetActive(false);
        }
    }

    public void HideStatus()
    {
        statusIcon.gameObject.SetActive(false);
    }
}