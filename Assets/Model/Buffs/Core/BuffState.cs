using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public abstract class BuffState
{
    public BuffType buffType { get; private set; }
    protected ScriptableBuffStateInfo StateInfo { get; private set; }

    protected BuffSaveInfo SaveInfo { get; private set; }

    public BuffState(ScriptableBuffStateInfo buffInfo, BuffSaveInfo saveInfo)
    {
        StateInfo = buffInfo;
        buffType = buffInfo.BuffType;
        SaveInfo = saveInfo;
    }

    public BuffState(ScriptableBuffStateInfo buffInfo)
    {
        StateInfo = buffInfo;
        buffType = buffInfo.BuffType;
    }

    public abstract void Apply(PotWithPlant plant);
    public abstract void Complete(PotWithPlant plant);
    public abstract BuffSaveInfo GetSaveInfo();
}
