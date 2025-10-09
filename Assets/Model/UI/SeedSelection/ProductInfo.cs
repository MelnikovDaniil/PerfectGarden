using System;
using UnityEngine;

public class ProductInfo
{
    public ProductInfo(object product,
        string name,
        Sprite preview,
        int price,
        int itemsAvaliable,
        bool isUnlocked,
        bool isVisible)
    {
        this.product = product;
        Name = name;
        Preview = preview;
        Price = price;
        IsUnlocked = isUnlocked;
        IsPartiallyVisible = isVisible;

        ItemsAvaliable = itemsAvaliable;
    }

    public string Name { get; }
    public Sprite Preview { get; }
    public int Price { get; }
    public bool IsUnlocked { get; set; }
    public bool IsPartiallyVisible { get; set; }
    public int ItemsAvaliable { get; set; }

    public T GetProduct<T>()
    {
        if (product.GetType() == typeof(T))
        {
            return (T)product;
        }
        else
        {
            throw new Exception("Product type is nop appropriate");
        }
    }

    private readonly object product;
}
