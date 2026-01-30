using Assets.Scripts.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public int stateCheckIntervalSec = 60;
    public GameObject GardenCanvas;
    public GameObject OrderCanvas;

    public GameObject notification;
    private bool wasGardenCanvasActive;
    private bool wasOrderCanvasActive;
    private CancellationTokenSource tokenSource;

    private void Awake()
    {
        Action openMenu = () =>
        {
            CameraManager.Instanse.SwitchCamera(true);
            wasGardenCanvasActive = GardenCanvas.gameObject.activeSelf;
            wasOrderCanvasActive = OrderCanvas.gameObject.activeSelf;

            GardenCanvas.gameObject.SetActive(false);
            OrderCanvas.gameObject.SetActive(false);
        };

        Action closeMenu = () =>
        {
            CameraManager.Instanse.SwitchCamera(false);
            GardenCanvas.gameObject.SetActive(wasGardenCanvasActive);
            OrderCanvas.gameObject.SetActive(wasOrderCanvasActive);
        };
        CareManager.OnCareMenuOpen += openMenu;
        PlantingManager.OnPlantingMenuOpen += openMenu;
        CareManager.OnCareMenuClosed += closeMenu;
        PlantingManager.OnPlantingMenuClosed += closeMenu;
        OrderManager.OnOrderAdded += (order) => notification.SetActive(true);
        OrderManager.OnAllOrdersComplete += () => notification.SetActive(false);

        if (!GuideMapper.IsGuideComplete(GuideStep.OrderManagement))
        {
            var orderMenuButton = GardenCanvas.transform.GetChild(0).gameObject;
            orderMenuButton.SetActive(false);
            GardernManager.OnGardenOpen += () =>
            {
                if (GuideMapper.GetGuideProgress(GuideStep.OrderManagement) >= 1)
                {
                    orderMenuButton.SetActive(true);
                    if (!GuideMapper.IsGuideComplete(GuideStep.OrderManagement))
                    {
                        _ = TutorialManager.Instance.SetTap(GardenCanvas.transform.GetChild(0).gameObject, true);
                    }
                }
            };

            OrderManager.OnGardenOpen += async () =>
            {
                if (!GuideMapper.IsGuideComplete(GuideStep.OrderManagement))
                {
                    await Task.Yield();
                    await TutorialManager.Instance.SetTap(OrderManagerUI.Instance.careButton.gameObject, true);
                };
            };
            OrderManager.OnOrderСomplete += (order) =>
            {
                GuideMapper.Complete(GuideStep.OrderManagement);
            };
        }
    }

    private async void Start()
    {
        tokenSource = new CancellationTokenSource();
        await StateChecking(tokenSource.Token);
    }

    public void OpenGardenLocation()
    {
        notification.SetActive(false);
        GardernManager.Instance.Open();
        GardenCanvas.SetActive(true);

        OrderManager.Instance.Close();
        OrderCanvas.SetActive(false);
    }

    public async void OpenOrderLocationAsync()
    {
        notification.SetActive(false);
        GardernManager.Instance.Close();
        GardenCanvas.SetActive(false);

        await OrderManager.Instance.OpenAsync();
        OrderCanvas.SetActive(true);
    }

    private async Task StateChecking(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(1000 * stateCheckIntervalSec);
            CareManager.Instance.GenerateCare(GardernManager.Instance.growingPlants);
            CareManager.Instance.UpdateMenu();
            GardernManager.Instance.UpdateMenu();
        }
    }

    private void OnApplicationQuit()
    {
        tokenSource.Cancel();
    }
}
