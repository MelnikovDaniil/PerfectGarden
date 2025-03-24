using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public List<ScriptableBuffStateInfo> buffs;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyBuffs(PotWithPlant plant, IEnumerable<BuffSaveInfo> buffSaves)
    {
        foreach (var save in buffSaves)
        {
            var scriptableBuff = buffs.FirstOrDefault(x => x.BuffType == save.buffType);
            scriptableBuff.Apply(plant, save);
        }
    }
}
