using UnityEngine;

[CreateAssetMenu(fileName = "WateringStateCareStateInfo", menuName = "States/WateringCareState")]
public class WateringScriptableCareStateInfo : WateringScriptableStateInfo<CareEvent>
{
    public override CareEvent EvenName => CareEvent.Watering;
}
