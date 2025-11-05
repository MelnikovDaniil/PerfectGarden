using System;
using UnityEngine;

public class PlantContext
{
    public PotWithPlant PotWithPlant;
    public PlantInfo plantInfo;

    public Action OnPlantingFinished { get; set; }
}
