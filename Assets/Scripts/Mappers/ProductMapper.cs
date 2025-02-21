using UnityEngine;

public static class ProductMapper
{
    private const string MapperName = "Product";

    public static int GetAvaliableProducts(string productName)
    {
        return PlayerPrefs.GetInt($"{MapperName}{productName}Avaliable", 0);
    }
}
