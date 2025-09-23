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

    public static int GetHighestReward()
    {
        return PlayerPrefs.GetInt(MapperName + "Record", 0);
    }

    public static bool TrySetNewHighestReward(int money)
    {
        var existingRecord = PlayerPrefs.GetInt(MapperName + "Record", 0);
        if (money > existingRecord)
        {
            Debug.Log($"New record {money}");
            PlayerPrefs.SetInt(MapperName + "Record", money);
            return true;
        }
        return false;
    }
}
