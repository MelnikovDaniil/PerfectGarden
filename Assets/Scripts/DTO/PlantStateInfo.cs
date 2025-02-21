using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlantStateInfo
{
    public string plantId;
    public Vector3 cellPosition;
    public string plantName;
    public string potName;
    public int currentStage;
    public List<CareEvent> waitingCareEvents;
    public long lastCareTimeUtc;
    public long lastStatusUpdateTimeUtc;
}
