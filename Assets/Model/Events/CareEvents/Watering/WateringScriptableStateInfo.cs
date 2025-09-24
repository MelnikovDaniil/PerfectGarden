using UnityEngine;

[CreateAssetMenu(fileName = "WateringStateStateInfo", menuName = "States/WateringState")]
public class WateringStateScriptableStateInfo : ScriptableCareStateInfo
{
    public override CareEvent EvenName => CareEvent.Watering;

    [Space]
    public float targetWateringParticlesAmount = 70;
    public float targetWateringBrightness = 0.5f;
    public ParticleSystem DustParticlesPrefab;
    public Color dryColor = new Color(1.5f, 1.5f, 1.5f);

    public override CareState CreateState()
    {
        return new WateringState(this);
    }
}
