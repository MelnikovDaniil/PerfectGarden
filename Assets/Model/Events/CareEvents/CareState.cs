public abstract class CareState
{
    public CareEvent eventName { get; private set; }
    protected ScriptableCareStateInfo StateInfo { get; private set; }

    public CareState(ScriptableCareStateInfo stateInfo)
    {
        StateInfo = stateInfo;
        eventName = stateInfo.EvenName;
    }

    public abstract void Apply(PotWithPlant plant);
    public abstract void Complete(PotWithPlant plant);
}
