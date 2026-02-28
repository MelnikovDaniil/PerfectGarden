using UnityEngine;

[CreateAssetMenu(fileName = "HydrationStateInfo", menuName = "States/Hydration")]
public class HydrationScriptableStateInfo : ScriptableCareStateInfo
{
    public override CareEvent EvenName => CareEvent.Hydration;

    public Material DustMaterial;
    public ParticleSystem DustParticlesPrefab;
    public AudioClip dustSplashClip;
    public ParticleSystem dustSplashPrefab;
    public Vector3 dustOffset = new Vector3(0, 0.2f, 0);

    public override State<CareEvent> CreateState()
    {
        return new HydrationState(this);
    }
}
