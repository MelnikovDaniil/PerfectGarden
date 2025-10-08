using System;
using UnityEngine;

public class PlantContext
{
    public PotWithPlant PotWithPlant;
    public PlantInfo plantInfo;
    public Sprite PackageSprite;
    public Sprite SeedSprite;

    public Action OnPlantingFinished { get; set; }
}
