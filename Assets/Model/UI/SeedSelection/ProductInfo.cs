﻿using System;
using UnityEngine;

public class ProductInfo
{
    public ProductInfo(object product, string name, Sprite preview, int price, int itemsAvaliable)
    {
        this.product = product;
        Name = name;
        Preview = preview;
        Price = price;
        ItemsAvaliable = itemsAvaliable;
    }

    public string Name { get; }
    public Sprite Preview { get; }
    public int Price { get; }
    public int ItemsAvaliable { get; }

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
