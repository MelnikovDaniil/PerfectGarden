using UnityEngine;

public abstract class ScriptableCareStateInfo : ScriptableObject
{
    public abstract CareState CreateState();
    public abstract CareEvent EvenName { get; }

    public void Apply(PotWithPlant plant)
    {
        var state = CreateState();
        plant.AddCareState(state);
    }

    public void Complete(PotWithPlant plant)
    {
        plant.CompleteState(EvenName);
    }
}
