using System;
using UnityEngine;

public static class ProductMapper
{
    private const string MapperName = "Product";

    public static int GetAvaliableProducts(string productName)
    {
        return PlayerPrefs.GetInt($"{MapperName}{productName}Avaliable", 0);
    }

    public static void Add(string productName)
    {
        var availableItems = GetAvaliableProducts(productName);
        PlayerPrefs.SetInt($"{MapperName}{productName}Avaliable", ++availableItems);
    }

    internal static void Remove(string productName)
    {
        var availableItems = GetAvaliableProducts(productName);
        PlayerPrefs.SetInt($"{MapperName}{productName}Avaliable", --availableItems);
    }
}
