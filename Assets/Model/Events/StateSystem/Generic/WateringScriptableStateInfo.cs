using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WateringStateStateInfo", menuName = "States/WateringState")]
public abstract class WateringScriptableStateInfo<TEvent> : ScriptableStateInfo<TEvent>
    where TEvent : Enum
{
    [Space]
    public float targetWateringParticlesAmount = 70;
    public float targetWateringBrightness = 0.5f;
    public ParticleSystem DustParticlesPrefab;
    public Color dryColor = new Color(1f, 1f, 1f);

    public override State<TEvent> CreateState()
    {
        return new WateringState<TEvent>(this);
    }
}
