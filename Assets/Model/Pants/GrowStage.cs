using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GrowStage
{
    public Renderer stagePlant;

    public List<CareEvent> requiredEvents;

    public List<OptionalCareEvent> optionalEvents;
}
