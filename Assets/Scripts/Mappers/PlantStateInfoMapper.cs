using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlantStateInfoMapper
{
    private const string MapperName = "PlantStateInfo";

    public static PlantStateInfo GetPlantState(Guid plantId)
    {
        return JsonUtility.FromJson<PlantStateInfo>(PlayerPrefs.GetString(MapperName + plantId));
    }

    public static void SavePlantState(PlantStateInfo plantState)
    {
        PlayerPrefs.SetString(MapperName + plantState.plantId, JsonUtility.ToJson(plantState));
    }

    public static List<PlantStateInfo> GetAllPlantStates()
    {
        var json = PlayerPrefs.GetString(MapperName);
        if (!string.IsNullOrEmpty(json))
        {
            return JsonHelper.FromJson<PlantStateInfo>(json).ToList();
        }
        return new PlantStateInfo[0].ToList();
    }

    public static void SavePlantStates(List<PlantStateInfo> plantStates)
    {
        PlayerPrefs.SetString(MapperName, JsonHelper.ToJson(plantStates.ToArray()));
    }
}
