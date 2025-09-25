using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class State<TEvent> : IState where TEvent : Enum
{
    public TEvent EventName { get; private set; }
    Enum IState.EventName => (Enum)(object)EventName;

    protected ScriptableStateInfo<TEvent> StateInfo { get; private set; }

    public State(ScriptableStateInfo<TEvent> stateInfo)
    {
        StateInfo = stateInfo;
        EventName = stateInfo.EvenName;
    }

    public abstract void Apply(PotWithPlant plant);
    public abstract void Complete(PotWithPlant plant);
    public Type GetEventType() => typeof(TEvent);
}

public interface IState
{
    Enum EventName { get; }
    void Apply(PotWithPlant plant);
    void Complete(PotWithPlant plant);
    Type GetEventType();
}