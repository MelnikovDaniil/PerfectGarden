using UnityEngine;

public static class MoneyMapper
{
    public static int Money
    {
        get => Get();
        set => Set(value);
    }

    private const string MapperName = "Money";

    private static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 0);
    }

    private static void Set(int money)
    {
        PlayerPrefs.SetInt(MapperName, money);
    }
}
