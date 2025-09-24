using System;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class ScriptableStateInfo<TEvent> : ScriptableObject
    where TEvent : Enum
{
    public abstract State<TEvent> CreateState();
    public abstract TEvent EvenName { get; }

    public void Apply(PotWithPlant plant)
    {
        var state = CreateState();
        plant.AddState(state);
    }

    public void Complete(PotWithPlant plant)
    {
        plant.CompleteState(EvenName);
    }
}