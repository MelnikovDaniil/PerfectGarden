using UnityEngine;

[CreateAssetMenu(fileName = "PestsStateInfo", menuName = "States/Pests")]
public class PestsScriptableStateInfo : ScriptableCareStateInfo
{
    public Worm wormPrefab;

    public int minWormNumber = 2;
    public int maxWormNumber = 4;

    public float minAppearanceRate = 0.5f;
    public float maxAppearanceRate = 2f;

    public float minAppearanceTime = 0.5f;
    public float maxAppearanceTime = 2f;

    public float wormGroupRadius = 0.2f;

    public override CareEvent EvenName => CareEvent.Pests;

    public override CareState CreateState()
    {
        return new PestsState(this);
    }
}
