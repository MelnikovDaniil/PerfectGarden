using System;
using System.Collections.Generic;

[Serializable]
public class OrderSaveInfo
{
    public string orderName;
    public string description;
    public string plantName;
    public string potName;
    public int reward;
    public int stage;
    public List<CareEvent> waitingCareEvents;
}
