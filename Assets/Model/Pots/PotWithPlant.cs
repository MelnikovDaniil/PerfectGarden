using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotWithPlant : MonoBehaviour
{
    public event Action OnSeedPlant;
    public event Action OnStageChange;
    public event Action OnEnableOnes;

    public Collider dirtCollider;

    public bool IsOrder;

    public bool IsDied;
    public bool IsLastStage => plantInfo.growStages.Count == currentStage + 1;
    public bool IsShouldBeRotted => !IsOrder && DateTime.UtcNow.Date - lastStageChangeTime.Date >= TimeSpan.FromDays(3);

    /// <summary>
    /// If it is order
    /// If plant was upgraded to new stage it will wait for timeBetweenStages to add new care events
    /// If plant was upgraded to new stage, has care and waiting for a one day, it will take more care events
    /// </summary>
    public bool IsNewCare()
    {
        if (IsOrder)
        {
            return true;
        }

        var speedGroBuff  = GetBuffState<SpeedGroBuffState>(BuffType.SpeedGro);
        //var timeBetweenStages = plantInfo.timeBetweenStages * ( != null ? 0.5f : 1f);
        var nextStageDate = speedGroBuff != null ? speedGroBuff.endDate : lastStageChangeTime + plantInfo.timeBetweenStages;
        return (nextStageDate < DateTime.UtcNow
            && (!waitingCareEvents.Any() /*&& lastCareAddedTime + plantInfo.timeBetweenStages < DateTime.UtcNow*/)
                || (waitingCareEvents.Any() && lastCareAddedTime + TimeSpan.FromDays(1) < DateTime.UtcNow));
    }

    [NonSerialized] public Renderer plantRenderer;
    [NonSerialized] public PotDirtFilling potDirtFilling;

    [NonSerialized] public PlantInfo plantInfo;
    [NonSerialized] public PotInfo potInfo;

    // To see days before death: if Now - lastStatusUpdate > 3
    [NonSerialized] public DateTime lastStageChangeTime;

    // To see, when new careEvents added time
    [NonSerialized] public DateTime lastCareAddedTime;
    [NonSerialized] public Vector3 cell;

    [NonSerialized] public List<CareEvent> waitingCareEvents = new List<CareEvent>();

    [Space]
    public Transform plantPlace;

    [Space]
    public float potBottomPostionY = -0.5f;

    [NonSerialized]
    public int currentStage;

    private List<IState> states;
    private List<BuffState> buffStates;


    private void Awake()
    {
        plantPlace.gameObject.SetActive(false);
        states = new List<IState>();
        buffStates = new List<BuffState>();
        potDirtFilling = GetComponent<PotDirtFilling>();
        dirtCollider.GetComponent<MeshRenderer>().material.color = new Color(0.25f, 0.25f, 0.25f);
    }

    private void OnEnable()
    {
        OnEnableOnes?.Invoke();
        OnEnableOnes = null;
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
        OnStageChange?.Invoke();
        OnStageChange = null;
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

    #region State
    public void AddState<TEvent>(State<TEvent> state) where TEvent : Enum
    {
        states.Add(state);
        state.Apply(this);
    }

    public T GetState<T>() where T : IState
    {
        return (T)states.Find(x => x is T);
    }

    public T GetState<T, TEvent>(TEvent careEvent) where T : State<TEvent> where TEvent : Enum
    {
        return (T)states.Find(x => x is State<TEvent> state &&
                                      state.EventName.Equals(careEvent));
    }

    public IState GetState<TEvent>(TEvent careEvent) where TEvent : Enum
    {
        return states.Find(x => x is State<TEvent> state &&
                                   state.EventName.Equals(careEvent));
    }

    public IState GetState(Enum careEvent)
    {
        return states.Find(x => x.EventName.Equals(careEvent));
    }

    ///// <summary>
    ///// Only searching by CareEvents
    ///// </summary>
    ///// <param name="careEvent"></param>
    ///// <returns></returns>
    //public CareState GetState(CareEvent careEvent)
    //{
    //    return (CareState)states.Find(x => x.EventName.Equals(careEvent));
    //}

    public void CompleteState<TEvent>(TEvent careEvent) where TEvent : Enum
    {
        var state = states.Find(x => x is State<TEvent> state &&
                                        state.EventName.Equals(careEvent)) as State<TEvent>;
        if (state != null)
        {
            state.Complete(this);
            states.Remove(state);
        }
    }

    public void CompleteState(Enum careEvent)
    {
        var state = states.Find(x => x.EventName.Equals(careEvent));
        if (state != null)
        {
            state.Complete(this);
            states.Remove(state);
        }
    }

    public void CompleteAllStates()
    {
        states.ForEach(x => x.Complete(this));
        states.Clear();
    }

    #endregion

    #region BuffState
    public void AddBuffState(BuffState state)
    {
        buffStates.Add(state);
        state.Apply(this);
    }

    public IEnumerable<BuffState> GetAllBuffStates()
    {
        return buffStates;
    }


    public T GetBuffState<T>() where T : BuffState
    {
        return (T)buffStates.Find(x => x.GetType() == typeof(T));
    }

    public T GetBuffState<T>(BuffType buffType) where T : BuffState
    {
        return (T)buffStates.Find(x => x.buffType == buffType);
    }

    public BuffState GetBuffState(BuffType buffType)
    {
        return buffStates.Find(x => x.buffType == buffType);
    }

    public void CompleteBuffState(BuffType buffType)
    {
        var state = buffStates.Find(x => x.buffType == buffType);
        state.Complete(this);
        buffStates.Remove(state);
    }

    public void CompleteAllBUffStates()
    {
        buffStates.ForEach(x => x.Complete(this));
        buffStates.Clear();
    }

    #endregion



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "seed")
        {
            collision.GetComponent<Seed>().Plant();
            OnSeedPlant?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        var y = potBottomPostionY * transform.localScale.y;
        Gizmos.DrawLine(new Vector3(-1f, y) + transform.position, new Vector3(1f, y) + transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(dirtCollider.bounds.center, dirtCollider.bounds.size);
    }
}
