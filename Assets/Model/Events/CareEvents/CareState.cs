public abstract class CareState : State<CareEvent>
{
    public CareState(ScriptableCareStateInfo stateInfo) : base(stateInfo)
    {
    }
}
