using System.Threading.Tasks;
using UnityEngine;

public abstract class ScriptableBuffStateInfo : ScriptableObject
{
    public abstract BuffState CreateState();
    public abstract BuffState CreateStateFromSave(BuffSaveInfo buffStateInfo);
    public abstract BuffType BuffType { get; }

    public void Apply(PotWithPlant plant)
    {
        var state = CreateState();
        plant.AddBuffState(state);
    }

    public void Apply(PotWithPlant plant, BuffSaveInfo buffStateInfo)
    {
        var state = CreateStateFromSave(buffStateInfo);
        plant.AddBuffState(state);
    }

    public void Complete(PotWithPlant plant)
    {
        plant.CompleteBuffState(BuffType);
    }
}
