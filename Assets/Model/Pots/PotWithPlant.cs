using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotWithPlant : MonoBehaviour
{
    public event Action OnSeedPlant;

    public Collider dirtCollider;

    public bool IsDied;
    public bool IsLastStage => plantInfo.growStages.Count == currentStage + 1;
    public bool IsShouldBeRotted => DateTime.UtcNow.Date - lastStageChangeTime.Date >= TimeSpan.FromDays(3);

    /// <summary>
    /// If plant was upgraded to new stage it will wait for timeBetweenStages to add new care events
    /// If plant was upgraded to new stage, has care and waiting for a one day, it will take more care events
    /// </summary>
    public bool IsNewCare => DateTime.UtcNow - lastStageChangeTime >= plantInfo.timeBetweenStages
                && (!waitingCareEvents.Any() && DateTime.UtcNow - lastCareAddedTime >= plantInfo.timeBetweenStages)
                    || (waitingCareEvents.Any() && DateTime.UtcNow - lastCareAddedTime >= TimeSpan.FromDays(1));

    [NonSerialized] public Renderer plantRenderer;
    [NonSerialized] public PotDirtFilling potDirtFilling;
    [NonSerialized] public PotWatering potWatering;

    [NonSerialized] public PlantInfo plantInfo;
    [NonSerialized] public PotInfo potInfo;

    // To see days before death: if Now - lastStatusUpdate > 3
    [NonSerialized] public DateTime lastStageChangeTime;

    // To see, when new careEvents added time
    [NonSerialized] public DateTime lastCareAddedTime;
    [NonSerialized] public Vector3 cell;

    /*[NonSerialized] */public List<CareEvent> waitingCareEvents = new List<CareEvent>();

    [Space]
    public Transform plantPlace;

    [Space]
    public float potBottomPostionY = -0.5f;

    [NonSerialized]
    public int currentStage;

    private List<CareState> careStates;


    private void Awake()
    {
        plantPlace.gameObject.SetActive(false);
        careStates = new List<CareState>();
        potDirtFilling = GetComponent<PotDirtFilling>();
        potWatering = GetComponent<PotWatering>();
    }

    public void SetStage(int plantStage)
    {
        currentStage = plantStage;
        plantPlace.gameObject.SetActive(true);
        foreach (Transform flower in plantPlace)
        {
            Destroy(flower.gameObject);
        }
        plantRenderer = Instantiate(plantInfo.growStages[plantStage].stagePlant, plantPlace.transform);
    }

    public void Rot()
    {
        IsDied = true;
        CompleteAllStates();
        plantPlace.gameObject.SetActive(true);
        foreach (Transform flower in plantPlace)
        {
            Destroy(flower.gameObject);
        }
        plantRenderer = Instantiate(plantInfo.rottenStage, plantPlace.transform);
    }

    public void UpdateStageChangeTime()
    {
        lastStageChangeTime = DateTime.UtcNow;
    }

    public void AddCareState(CareState state)
    {
        careStates.Add(state);
        state.Apply(this);
    }

    public T GetState<T>() where T : CareState
    {
        return (T)careStates.Find(x => x.GetType() == typeof(T));
    }

    public T GetState<T>(CareEvent careEvent) where T : CareState
    {
        return (T)careStates.Find(x => x.eventName == careEvent);
    }

    public CareState GetState(CareEvent careEvent)
    {
        return careStates.Find(x => x.eventName == careEvent);
    }

    public void CompleteState(CareEvent careEvent)
    {
        var state = careStates.Find(x => x.eventName == careEvent);
        state.Complete(this);
        careStates.Remove(state);
    }

    public void CompleteAllStates()
    {
        careStates.ForEach(x => x.Complete(this));
        careStates.Clear();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "seed")
        {
            OnSeedPlant?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        var y = potBottomPostionY * transform.localScale.y;
        Gizmos.DrawLine(new Vector3(-1f, y) + transform.position, new Vector3(1f, y) + transform.position);
    }
}
