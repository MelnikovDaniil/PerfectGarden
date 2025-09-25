using UnityEngine;

[CreateAssetMenu(fileName = "WateringStatePlantStateInfo", menuName = "States/WateringPlantState")]
public class WateringScriptablePlantStateInfo : WateringScriptableStateInfo<PlantingEvent>
{
    public override PlantingEvent EvenName => PlantingEvent.Watering;
}
