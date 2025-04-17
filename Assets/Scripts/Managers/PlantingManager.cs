using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlantingManager : MonoBehaviour
{
    public static event Action<PotWithPlant> OnPlantingFinished;
    public static PlantingManager Instance;
    public List<PlantingEvent> tempEvents;

    [SerializeField] private CareMenu CareCanvas;
    [SerializeField] private FinishPlanting finishPlantingCanvas;

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
            await eventHandler.PrepareAsync();
            await eventHandler.StartAsync();
            eventHandler.Clear();
        }

        finishPlantingCanvas.ShowMenu();
        finishPlantingCanvas.finishPlantingButton.onClick.RemoveAllListeners();
        finishPlantingCanvas.finishPlantingButton.onClick.AddListener(() =>
        {
            OnPlantingFinished?.Invoke(plantingContext.PotWithPlant);
            finishPlantingCanvas.HideMenu();
        });

        plantingContext.PotWithPlant.cell = tilePostion;
        plantingContext.PotWithPlant.transform.parent = null;
        plantingContext.PotWithPlant.gameObject.SetActive(true);
        plantingContext.PotWithPlant.SetStage(0);
        plantingContext.PotWithPlant.UpdateStageChangeTime();
    }
}
