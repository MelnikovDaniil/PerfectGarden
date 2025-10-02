using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlantingManager : MonoBehaviour
{
    public static Action OnPlantingMenuOpen;
    public static Action OnPlantingMenuClosed;
    public static event Action OnPlantingCancel;
    public static event Action<PotWithPlant> OnPlantingFinished;
    public static PlantingManager Instance;
    public List<PlantingEvent> tempEvents;
    public List<ScriptableStateInfo<PlantingEvent>> stateInfos;

    [SerializeField] private CareMenu CareCanvas;
    [SerializeField] private FinishPlanting finishPlantingCanvas;
    private CancellationTokenSource tokenSource;

    private List<PlantEventHandler> plantingEventHandlers;

    private void Awake()
    {
        Instance = this;
        plantingEventHandlers = GetComponents<PlantEventHandler>().ToList();
    }

    private async void Start()
    {
        await StartPlanting(Vector3Int.zero);
    }

    public async Task StartPlanting(Vector3 tilePostion)
    {
        OnPlantingMenuOpen?.Invoke();
        tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        var currentEventHandlers = new Queue<PlantEventHandler>();
        var plantingContext = new PlantContext();

        foreach (var eventName in tempEvents)
        {
            var eventHandler = plantingEventHandlers.First(x => x.EventName == eventName);
            currentEventHandlers.Enqueue(eventHandler);
            eventHandler.Setup(plantingContext);
        }

        foreach (var eventHandler in currentEventHandlers)
        {
            var stateInfo = stateInfos.Find(stateInfo => stateInfo.EvenName == eventHandler.EventName);
            stateInfo?.Apply(plantingContext.PotWithPlant);
            await eventHandler.PrepareAsync(token);
            await eventHandler.StartAsync(token);
            eventHandler.Clear();
            stateInfo?.Complete(plantingContext.PotWithPlant);

            if (token.IsCancellationRequested)
            {
                if (plantingContext.PotWithPlant?.gameObject != null)
                {
                    Destroy(plantingContext.PotWithPlant.gameObject);
                }

                finishPlantingCanvas.HideMenu();
                OnPlantingMenuClosed?.Invoke();
                OnPlantingCancel?.Invoke();
                return;
            }
        }

        finishPlantingCanvas.ShowMenu();
        finishPlantingCanvas.finishPlantingButton.onClick.RemoveAllListeners();
        finishPlantingCanvas.finishPlantingButton.onClick.AddListener(() =>
        {
            OnPlantingFinished?.Invoke(plantingContext.PotWithPlant);
            OnPlantingMenuClosed?.Invoke();
            finishPlantingCanvas.HideMenu();
        });

        plantingContext.PotWithPlant.cell = tilePostion;
        plantingContext.PotWithPlant.transform.parent = null;
        plantingContext.PotWithPlant.gameObject.SetActive(true);
        plantingContext.PotWithPlant.SetStage(0);
        plantingContext.PotWithPlant.UpdateStageChangeTime();
    }

    public void CancelPlanting()
    {
        tokenSource.Cancel();
        finishPlantingCanvas.HideMenu();
        OnPlantingCancel?.Invoke();
    }
}
