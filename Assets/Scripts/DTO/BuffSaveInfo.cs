using System;
using UnityEngine;

[Serializable]
public class BuffSaveInfo
{
    public BuffType buffType;

    public string info;

    public void Serialize()
    {
        info = JsonUtility.ToJson(this);
    }

    public T Deserialize<T>()
    {
        return JsonUtility.FromJson<T>(info);
    }
}
