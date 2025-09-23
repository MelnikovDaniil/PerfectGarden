using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class OrderMapper
{
    private const string MapperName = "Order";

    public static List<OrderSaveInfo> GetAllOrders()
    {
        var json = PlayerPrefs.GetString(MapperName);
        if (!string.IsNullOrEmpty(json))
        {
            return JsonHelper.FromJson<OrderSaveInfo>(json).ToList();
        }
        return new OrderSaveInfo[0].ToList();
    }

    public static void SaveOrders(List<OrderSaveInfo> orders)
    {
        PlayerPrefs.SetString(MapperName, JsonHelper.ToJson(orders.ToArray()));
        PlayerPrefs.Save();
    }

    public static DateTime GetLastGenerationTime()
    {
        var ticks = long.Parse(PlayerPrefs.GetString(MapperName + "LastGenerationTime", "0"));
        return ticks > 0 ? new DateTime(ticks) : DateTime.Now;
    }
    public static void SaveLastGenerationTime(DateTime lastOrderGenerationTime)
    {
        PlayerPrefs.SetString(MapperName + "LastGenerationTime", lastOrderGenerationTime.Ticks.ToString());
    }
}
