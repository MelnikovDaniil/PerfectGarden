using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeedStateInfo", menuName = "States/Weed")]
public class WeedScriptableStateInfo : ScriptableCareStateInfo
{
    public Weed weedPrefab;
    public List<Sprite> weedSprites;
    public int minWeedNumber = 2;
    public int maxWeedNumber = 4;

    public override CareEvent EvenName => CareEvent.Weed;

    public override CareState CreateState()
    {
        return new WeedCareState(this);
    }
}
