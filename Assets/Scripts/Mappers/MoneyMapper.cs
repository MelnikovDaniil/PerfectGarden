using UnityEngine;

public static class MoneyMapper
{
    public static int MaxMoneyReached {  get => GetMaxMoneyReached(); }
    private static int maxMoneyReached = 0;


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
        if (money > MaxMoneyReached) SetMaxMoneyReached(money);
    }

    public static int GetHighestReward()
    {
        return PlayerPrefs.GetInt(MapperName + "RewardRecord", 0);
    }

    public static bool TrySetNewHighestReward(int money)
    {
        var existingRecord = PlayerPrefs.GetInt(MapperName + "RewardRecord", 0);
        if (money > existingRecord)
        {
            Debug.Log($"New record record {money}");
            PlayerPrefs.SetInt(MapperName + "RewardRecord", money);
            return true;
        }
        return false;
    }

    private static void SetMaxMoneyReached(int money)
    {
        maxMoneyReached = money;
        PlayerPrefs.SetInt(MapperName + "MoneyReached", money);
    }

    private static int GetMaxMoneyReached()
    {
        if (maxMoneyReached == 0)
        {
            return PlayerPrefs.GetInt(MapperName + "MoneyReached", 0);
        }

        return maxMoneyReached;
    }
}
