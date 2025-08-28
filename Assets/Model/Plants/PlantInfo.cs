using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlantInfo", menuName = "ScriptableObjects/PlantInfo")]
public class PlantInfo : ScriptableObject
{
    public string name;

    public PlantType plantType;

    public List<PlantingEvent> plantingEvents;

    public List<GrowStage> growStages;

    public Renderer rottenStage;

    public TimeSpan timeBetweenStages = TimeSpan.FromSeconds(30);

    public Sprite seedSprite;

    public Sprite packageSprite;

    public Sprite shopSprite;

    public int seedPrice;
    public int plantPrice => seedPrice * 3;
}
