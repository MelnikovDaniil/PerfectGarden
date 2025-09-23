using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class CareManager : MonoBehaviour
{
    public static Action<PotWithPlant> OnCareFinished;
    public static Action OnCareMenuOpen;
    public static Action OnCareMenuClosed;

    public static CareManager Instance;

    public static bool CareInProcess;

    public List<ScriptableCareStateInfo> stateInfos;

    [SerializeField] private CareMenu CareCanvas;

    public Transform carePlace;
    public Transform plantPlace;

    private CancellationTokenSource eventCancellationSource;

    private PotWithPlant currentPlant;

    private List<CareEventHandler> careEventHandlers;
    private List<BuffEventHandler> buffEventHandlers;

    private void Awake()
    {
        Instance = this;
        careEventHandlers = GetComponents<CareEventHandler>().ToList();
        buffEventHandlers = GetComponents<BuffEventHandler>().ToList();
    }

    public int careEventStageAmount = 3;

    public void GenerateCare(List<PotWithPlant> potWithPlants)
    {
        foreach (var potWithPlant in potWithPlants.Where(x => !x.IsDied))
        {
            if (!potWithPlant.IsShouldBeRotted)

            {
                if (potWithPlant.IsNewCare() && !potWithPlant.IsLastStage)
                {
                    var plantStage = potWithPlant.plantInfo.growStages[potWithPlant.currentStage];

                    var newEvents = plantStage.requiredEvents.Where(x => !potWithPlant.waitingCareEvents.Contains(x)).ToList();
                    var numberOfEventsLeft = Mathf.Clamp(careEventStageAmount - newEvents.Count, 0, careEventStageAmount);

                    newEvents.AddRange(TakeRandomOptionalEvent(plantStage.optionalEvents, numberOfEventsLeft));
                    potWithPlant.waitingCareEvents.AddRange(newEvents);
                    potWithPlant.waitingCareEvents = potWithPlant.waitingCareEvents.Distinct().ToList();
                    potWithPlant.lastCareAddedTime = DateTime.UtcNow;
                }

                GenerateStates(potWithPlant);
            }
            else
            {
                potWithPlant.Rot();
            }
        }
    }

    public void GenerateStates(PotWithPlant potWithPlant)
    {
        foreach (var careEvent in potWithPlant.waitingCareEvents.Where(careEvent => potWithPlant.GetState(careEvent) == null))
        {
            stateInfos.Find(stateInfo => stateInfo.EvenName == careEvent)?.Apply(potWithPlant);

        }
    }

    private IEnumerable<CareEvent> TakeRandomOptionalEvent(List<OptionalCareEvent> careEvents, int amount)
    {
        if (careEvents == null)
        {
            throw new ArgumentNullException(nameof(careEvents));
        }

        var elements = careEvents.ToList();

        var result = new List<CareEvent>();
        var totalWeight = elements.Sum(e => e.chance);

        for (var i = 0; i < amount; i++)
        {
            var randomNumber = Random.Range(0, totalWeight);
            var selected = elements.FirstOrDefault(element =>
            {
                randomNumber -= element.chance;
                return randomNumber < 0;
            });

            result.Add(selected.careEvent);
        }

        return result;
    }

    public async Task MoveToTrash()
    {
        currentPlant.gameObject.SetActive(false);
        await Task.Delay(2000);
        FinishCare();
    }

    public async Task PurchaseAsync()
    {
        currentPlant.gameObject.SetActive(false);
        await Task.Delay(2000);
        MoneyMapper.Money += currentPlant.plantInfo.plantPrice;
        FinishCare();
    }


    public async Task OpenCareMenu(PotWithPlant potWithPlant)
    {
        CareInProcess = true;
        currentPlant = potWithPlant;

        SetupPlant(currentPlant);

        CareCanvas.ShowMenu(currentPlant);
        CareCanvas.OnBackPressed = FinishCare;
        OnCareMenuOpen.Invoke();
    }

    public void UpdateMenu()
    {
        if (CareInProcess)
        {
            CareCanvas.ShowMenu(currentPlant);
            CareCanvas.OnBackPressed = FinishCare;
        }
    }

    public async Task StartCareAsync(CareEvent eventName)
    {
        eventCancellationSource = new CancellationTokenSource();

        CareCanvas.HideAllButtons();
        CareCanvas.OnBackPressed = () =>
        {
            eventCancellationSource?.Cancel();
        };

        var eventHandler = careEventHandlers.First(x => x.EventName == eventName);
        var careContext = new CareContext
        {
            CarePlace = carePlace,
            PotWithPlant = currentPlant
        };

        eventHandler.Setup(careContext);
        await eventHandler.PrepareAsync(eventCancellationSource.Token);
        await eventHandler.StartAsync(eventCancellationSource.Token);
        eventHandler.Clear();
        await Task.Delay((int)(CameraManager.Instanse.transitionDuration * 1000));

        if (eventHandler.Status == HandlingStatus.Finished)
        {
            stateInfos.FirstOrDefault(stateInfo => stateInfo.EvenName == eventName)?.Complete(currentPlant);
            currentPlant.waitingCareEvents.Remove(eventName);

            if (!currentPlant.waitingCareEvents.Any())
            {
                if (!currentPlant.IsOrder)
                {
                    RewardManager.Instance.GenerateCareLargeReward();
                    currentPlant.UpdateStageChangeTime();
                    currentPlant.SetStage(++currentPlant.currentStage);
                }
                else
                {
                    FinishCare();
                    return;
                }
            }
            else
            {
                RewardManager.Instance.GenerateCareReward();
            }
        }

        SetupPlant(currentPlant);

        CareCanvas.ShowMenu(currentPlant);
        CareCanvas.OnBackPressed = FinishCare;
    }

    public async Task StartBuffAsync(BuffType buffType)
    {
        eventCancellationSource = new CancellationTokenSource();

        CareCanvas.HideAllButtons();
        CareCanvas.OnBackPressed = () =>
        {
            eventCancellationSource?.Cancel();
        };

        var eventHandler = buffEventHandlers.First(x => x.EventName == buffType);
        var careContext = new CareContext
        {
            CarePlace = carePlace,
            PotWithPlant = currentPlant
        };

        eventHandler.Setup(careContext);
        await eventHandler.PrepareAsync(eventCancellationSource.Token);
        await eventHandler.StartAsync(eventCancellationSource.Token);
        eventHandler.Clear();
        await Task.Delay((int)(CameraManager.Instanse.transitionDuration * 1000));
        if (eventHandler.Status == HandlingStatus.Finished)
        {
            BuffManager.Instance.ApplyBuff(currentPlant, buffType);
        }
        
        SetupPlant(currentPlant);

        CareCanvas.ShowMenu(currentPlant);
        CareCanvas.OnBackPressed = FinishCare;
    }

    private void FinishCare()
    {
        CareCanvas.HideMenu();
        OnCareFinished?.Invoke(currentPlant);
        CareInProcess = false;
        currentPlant.transform.parent = null;
        currentPlant = null;
        OnCareMenuClosed?.Invoke();
    }

    private void SetupPlant(PotWithPlant potWithPlant)
    {
        potWithPlant.transform.parent = plantPlace;
        potWithPlant.transform.localScale = Vector3.one;
        potWithPlant.transform.localPosition = Vector3.zero;
        potWithPlant.gameObject.SetActive(true);
    }
}
